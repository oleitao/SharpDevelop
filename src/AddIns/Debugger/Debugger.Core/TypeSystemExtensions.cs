﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil.Metadata;

namespace Debugger
{
	/// <summary>
	/// Cecil-based metadata loader.
	/// </summary>
	public static class TypeSystemExtensions
	{
		#region Module Loading
		static ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo> weakTable = new ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo>();
		
		class ModuleMetadataInfo
		{
			public readonly Module Module;
			public Dictionary<IUnresolvedEntity, uint> MetadataTokens = new Dictionary<IUnresolvedEntity, uint>();
			public Dictionary<uint, IUnresolvedMethod> TokenToMethod = new Dictionary<uint, IUnresolvedMethod>();
			public Dictionary<IUnresolvedMember, ITypeReference[]> LocalVariableTypes = new Dictionary<IUnresolvedMember, ITypeReference[]>();
			
			public ModuleMetadataInfo(Module module)
			{
				this.Module = module;
			}
			
			public void AddMethod(IUnresolvedMethod method, CecilLoader loader)
			{
				var cecilMethod = loader.GetCecilObject(method);
				uint token = cecilMethod.MetadataToken.ToUInt32();
				this.MetadataTokens[method] = token;
				this.TokenToMethod[token] = method;
				if (cecilMethod.HasBody)
					this.LocalVariableTypes[method] = cecilMethod.Body.Variables.Select(v => loader.ReadTypeReference(v.VariableType)).ToArray();
			}
		}
		
		internal static Task<IUnresolvedAssembly> LoadModuleAsync(Module module, ICorDebugModule corModule)
		{
			string name = corModule.GetName();
			if (corModule.IsDynamic() == 1 || corModule.IsInMemory() == 1)
				return Task.FromResult<IUnresolvedAssembly>(new DefaultUnresolvedAssembly(name));
			
			//return Task.FromResult(LoadModule(module, name));
			return Task.Run(() => LoadModule(module, name));
		}
		
		static IUnresolvedAssembly LoadModule(Module module, string name)
		{
			CecilLoader loader = new CecilLoader(true);
			loader.IncludeInternalMembers = true;
			var asm = loader.LoadAssemblyFile(name);
			var moduleMetadataInfo = new ModuleMetadataInfo(module);
			foreach (var typeDef in asm.GetAllTypeDefinitions()) {
				var cecilTypeDef = loader.GetCecilObject(typeDef);
				moduleMetadataInfo.MetadataTokens[typeDef] = cecilTypeDef.MetadataToken.ToUInt32();
				foreach (var member in typeDef.Fields) {
					var cecilMember = loader.GetCecilObject(member);
					moduleMetadataInfo.MetadataTokens[member] = cecilMember.MetadataToken.ToUInt32();
				}
				foreach (var member in typeDef.Methods) {
					moduleMetadataInfo.AddMethod(member, loader);
				}
				foreach (var member in typeDef.Properties) {
					if (member.CanGet)
						moduleMetadataInfo.AddMethod(member.Getter, loader);
					if (member.CanSet)
						moduleMetadataInfo.AddMethod(member.Setter, loader);
				}
				foreach (var member in typeDef.Events) {
					if (member.CanAdd)
						moduleMetadataInfo.AddMethod(member.AddAccessor, loader);
					if (member.CanRemove)
						moduleMetadataInfo.AddMethod(member.RemoveAccessor, loader);
					if (member.CanInvoke)
						moduleMetadataInfo.AddMethod(member.InvokeAccessor, loader);
				}
			}
			weakTable.Add(asm, moduleMetadataInfo);
			return asm;
		}
		
		static ModuleMetadataInfo GetInfo(IAssembly assembly)
		{
			ModuleMetadataInfo info;
			if (!weakTable.TryGetValue(assembly.UnresolvedAssembly, out info))
				throw new ArgumentException("The assembly was not from the debugger type system");
			return info;
		}
		#endregion
		
		#region GetModule / GetMetadataToken / GetVariableType
		public static Module GetModule(this IAssembly assembly)
		{
			return GetInfo(assembly).Module;
		}
		
		public static uint GetMetadataToken(this ITypeDefinition typeDefinition)
		{
			var info = GetInfo(typeDefinition.ParentAssembly);
			return info.MetadataTokens[typeDefinition.Parts[0]];
		}
		
		public static uint GetMetadataToken(this IField field)
		{
			var info = GetInfo(field.ParentAssembly);
			return info.MetadataTokens[field.UnresolvedMember];
		}
		
		public static uint GetMetadataToken(this IMethod method)
		{
			var info = GetInfo(method.ParentAssembly);
			return info.MetadataTokens[method.UnresolvedMember];
		}
		
		public static IType GetLocalVariableType(this IMethod method, int index)
		{
			var info = GetInfo(method.ParentAssembly);
			var variableTypes = info.LocalVariableTypes[method.UnresolvedMember];
			return variableTypes[index].Resolve(method.Compilation);
		}
		#endregion
		
		#region IType -> ICorDebugType
		public static ICorDebugType ToCorDebug(this IType type)
		{
			AppDomain appDomain;
			return ToCorDebug(type, out appDomain);
		}
		
		static ICorDebugType ToCorDebug(IType type, out AppDomain appDomain)
		{
			switch (type.Kind) {
				case TypeKind.Class:
				case TypeKind.Interface:
				case TypeKind.Struct:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Module:
				case TypeKind.Void:
					return ConvertTypeDefOrParameterizedType(type, out appDomain);
				case TypeKind.Array:
					{
						var arrayType = (ArrayType)type;
						var elementType = ToCorDebug(arrayType.ElementType, out appDomain);
						return appDomain.CorAppDomain2.GetArrayOrPointerType(
							(uint)(arrayType.Dimensions == 1 ? CorElementType.SZARRAY : CorElementType.ARRAY),
							(uint)arrayType.Dimensions, elementType);
					}
				case TypeKind.Pointer:
				case TypeKind.ByReference:
					{
						var pointerType = (TypeWithElementType)type;
						var elementType = ToCorDebug(pointerType.ElementType, out appDomain);
						return appDomain.CorAppDomain2.GetArrayOrPointerType(
							(uint)(type.Kind == TypeKind.Pointer ? CorElementType.PTR : CorElementType.BYREF),
							0, elementType);
					}
				default:
					throw new System.Exception("Invalid value for TypeKind");
			}
		}
		
		static ICorDebugType ConvertTypeDefOrParameterizedType(IType type, out AppDomain appDomain)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			appDomain = GetAppDomain(typeDef.Compilation);
			var info = GetInfo(typeDef.ParentAssembly);
			uint token = info.MetadataTokens[typeDef.Parts[0]];
			ICorDebugClass corClass = info.Module.CorModule.GetClassFromToken(token);
			List<ICorDebugType> corGenArgs = new List<ICorDebugType>();
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null) {
				foreach (var typeArg in pt.TypeArguments) {
					corGenArgs.Add(typeArg.ToCorDebug());
				}
			}
			return ((ICorDebugClass2)corClass).GetParameterizedType((uint)(type.IsReferenceType == false ? CorElementType.VALUETYPE : CorElementType.CLASS), corGenArgs.ToArray());
		}
		#endregion
		
		#region Compilation
		class DebugCompilation : SimpleCompilation
		{
			public readonly AppDomain AppDomain;
			
			public DebugCompilation(AppDomain appDomain, IUnresolvedAssembly mainAssembly, IEnumerable<IAssemblyReference> assemblyReferences)
				: base(mainAssembly, assemblyReferences)
			{
				this.AppDomain = appDomain;
			}
		}
		
		internal static ICompilation CreateCompilation(AppDomain appDomain, IList<IUnresolvedAssembly> assemblies)
		{
			if (assemblies.Count == 0)
				return new DebugCompilation(appDomain, MinimalCorlib.Instance, Enumerable.Empty<IAssemblyReference>());
			else
				return new DebugCompilation(appDomain, assemblies[0], assemblies.Skip(1));
		}
		
		public static AppDomain GetAppDomain(this ICompilation compilation)
		{
			DebugCompilation dc = compilation as DebugCompilation;
			if (dc != null)
				return dc.AppDomain;
			else
				throw new InvalidOperationException("The compilation is not a debugger type system");
		}
		
		public static IType Import(this ICompilation compilation, ICorDebugType corType)
		{
			return ToTypeReference(corType, GetAppDomain(compilation).Process).Resolve(compilation.TypeResolveContext);
		}
		#endregion
		
		#region ICorDebugType -> IType
		public static ITypeReference ToTypeReference(this ICorDebugType corType, Process process)
		{
			switch ((CorElementType)corType.GetTheType()) {
				case CorElementType.VOID:
					return KnownTypeReference.Void;
				case CorElementType.BOOLEAN:
					return KnownTypeReference.Boolean;
				case CorElementType.CHAR:
					return KnownTypeReference.Char;
				case CorElementType.I1:
					return KnownTypeReference.SByte;
				case CorElementType.U1:
					return KnownTypeReference.Byte;
				case CorElementType.I2:
					return KnownTypeReference.Int16;
				case CorElementType.U2:
					return KnownTypeReference.UInt16;
				case CorElementType.I4:
					return KnownTypeReference.Int32;
				case CorElementType.U4:
					return KnownTypeReference.UInt32;
				case CorElementType.I8:
					return KnownTypeReference.Int64;
				case CorElementType.U8:
					return KnownTypeReference.UInt64;
				case CorElementType.R4:
					return KnownTypeReference.Single;
				case CorElementType.R8:
					return KnownTypeReference.Double;
				case CorElementType.STRING:
					return KnownTypeReference.String;
				case CorElementType.PTR:
					return new PointerTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.BYREF:
					return new ByReferenceTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
					// Get generic arguments
					List<ITypeReference> genericArguments = new List<ITypeReference>();
					foreach (ICorDebugType t in corType.EnumerateTypeParameters().GetEnumerator()) {
						genericArguments.Add(t.ToTypeReference(process));
					}
					var module = process.GetModule(corType.GetClass().GetModule());
					ITypeReference typeDefinitionReference = ToTypeDefinitionReference(module, corType.GetClass().GetToken());
					if (genericArguments.Count > 0)
						return new ParameterizedTypeReference(typeDefinitionReference, genericArguments);
					else
						return typeDefinitionReference;
				case CorElementType.ARRAY:
					return new ArrayTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process),
					                              (int)corType.GetRank());
				case CorElementType.GENERICINST:
					throw new NotSupportedException();
				case CorElementType.I:
					return KnownTypeReference.IntPtr;
				case CorElementType.U:
					return KnownTypeReference.UIntPtr;
				case CorElementType.OBJECT:
					return KnownTypeReference.Object;
				case CorElementType.SZARRAY:
					return new ArrayTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.CMOD_REQD:
				case CorElementType.CMOD_OPT:
					return corType.GetFirstTypeParameter().ToTypeReference(process);
				default:
					throw new InvalidOperationException("Invalid value for CorElementType");
			}
		}
		
		static ITypeReference ToTypeDefinitionReference(Module module, uint classToken)
		{
			var props = module.MetaData.GetTypeDefProps(classToken);
			var visibility = (TypeAttributes)props.Flags & TypeAttributes.VisibilityMask;
			if (visibility == TypeAttributes.Public || visibility == TypeAttributes.NotPublic) {
				// top-level type
				int dot = props.Name.LastIndexOf('.');
				int tick = props.Name.LastIndexOf('`');
				string ns = dot > 0 ? props.Name.Substring(0, dot) : string.Empty;
				string name;
				int typeParameterCount;
				if (tick < 0) {
					name = props.Name.Substring(dot + 1);
					typeParameterCount = 0;
				} else {
					name = props.Name.Substring(dot + 1, tick - (dot + 1));
					int.TryParse(props.Name.Substring(tick + 1), out typeParameterCount);
				}
				return new GetClassTypeReference(ns, name, typeParameterCount);
			} else {
				// nested type
				uint enclosingTk = module.MetaData.GetNestedClassProps(classToken).EnclosingClass;
				var declaringTypeReference = ToTypeDefinitionReference(module, enclosingTk);
				int typeParameterCount;
				string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(props.Name, out typeParameterCount);
				return new NestedTypeReference(declaringTypeReference, name, typeParameterCount);
			}
		}
		#endregion
		
		public static bool IsPrimitiveType(this IType type)
		{
			var def = type.GetDefinition();
			if (def != null) {
				switch (def.KnownTypeCode) {
					case KnownTypeCode.Boolean:
					case KnownTypeCode.Char:
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
					case KnownTypeCode.Single:
					case KnownTypeCode.Double:
						return true;
				}
			}
			return false;
		}
		
		public static bool IsInteger(this IType type)
		{
			var def = type.GetDefinition();
			if (def != null) {
				switch (def.KnownTypeCode) {
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
						return true;
				}
			}
			return false;
		}
		
		public static bool IsKnownType(this IType type, KnownTypeCode knownType)
		{
			var def = type.GetDefinition();
			return def != null && def.KnownTypeCode == knownType;
		}
		
		public static IField GetBackingField(this IMethod method)
		{
			throw new NotImplementedException();
		}
		
		public static ICorDebugType[] GetTypeArguments(this IMethod method)
		{
			List<ICorDebugType> typeArgs = new List<ICorDebugType>();
			var specMethod = method as SpecializedMethod;
			if (specMethod != null) {
				if (specMethod.Substitution.ClassTypeArguments != null)
					typeArgs.AddRange(specMethod.Substitution.ClassTypeArguments.Select(t => t.ToCorDebug()));
				if (specMethod.Substitution.MethodTypeArguments != null)
					typeArgs.AddRange(specMethod.Substitution.MethodTypeArguments.Select(t => t.ToCorDebug()));
			}
			return typeArgs.ToArray();
		}
		
		public static ICorDebugFunction ToCorFunction(this IMethod method)
		{
			Module module = method.DeclaringType.GetDefinition().ParentAssembly.GetModule();
			return module.CorModule.GetFunctionFromToken(method.GetMetadataToken());
		}
		
		public static bool IsDisplayClass(this IType type)
		{
			if (type.Name.StartsWith("<>") && type.Name.Contains("__DisplayClass")) {
				return true; // Anonymous method or lambda
			}
			
			if (type.GetDefinition().Attributes.Any(a => a.AttributeType.FullName == typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute).FullName)) {
				if (type.GetAllBaseTypeDefinitions().Any(t => t.FullName == typeof(System.Collections.IEnumerator).FullName)) {
					return true; // yield
				}
				if (type.GetAllBaseTypeDefinitions().Any(t => t.FullName == "System.Runtime.CompilerServices.IAsyncStateMachine")) {
					return true; // async
				}
			}
			
			return false;
		}
		
		public static IMethod Import(this ICompilation compilation, ICorDebugFunction corFunction, List<ICorDebugType> typeArgs)
		{
			IMethod definition = Import(compilation, corFunction);
			if (typeArgs == null || typeArgs.Count == 0) {
				return definition;
			}
			int classTPC = definition.DeclaringTypeDefinition.TypeParameterCount;
			IType[] classTPs = typeArgs.Take(classTPC).Select(t => compilation.Import(t)).ToArray();
			IType[] methodTPs = null;
			if (definition.TypeParameters.Count > 0)
				methodTPs = typeArgs.Skip(classTPC).Select(t => compilation.Import(t)).ToArray();
			return new SpecializedMethod(definition, new TypeParameterSubstitution(classTPs, methodTPs));
		}
		
		public static IMethod Import(this ICompilation compilation, ICorDebugFunction corFunction)
		{
			Module module = compilation.GetAppDomain().Process.GetModule(corFunction.GetModule());
			var info = GetInfo(module.Assembly);
			var unresolved = info.TokenToMethod[corFunction.GetToken()];
			return unresolved.Resolve(new SimpleTypeResolveContext(module.Assembly));
		}
	}
}