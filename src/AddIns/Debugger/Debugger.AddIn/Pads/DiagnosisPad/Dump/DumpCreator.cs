using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Debugger.MiniDump;
using NLog;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump
{
	/// <summary>
	/// Generate dmp file
	/// </summary>
	public class DumpCreator
	{
		/// <summary>
		/// The logger
		/// </summary>
		static Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

		/// <summary>
		/// The dump extension
		/// </summary>
		const string DUMP_EXTENSION = ".dmp";

		/// <summary>
		/// The dump folder
		/// </summary>
		const string DUMP_FOLDER = @"Pads\DiagnosisPad\Dump\";

		/// <summary>
		/// The proc dump executable
		/// </summary>
		const string ProcDumpExe = "procdump.exe";
		/// <summary>
		/// The proc dump exe64a
		/// </summary>
		const string ProcDumpExe64a = "procdump64a.exe";
		/// <summary>
		/// The ps suspend
		/// </summary>
		const string PsSuspend = "pssuspend.exe";
		/// <summary>
		/// The ps suspend64
		/// </summary>
		const string PsSuspend64 = "pssuspend64.exe";
		/// <summary>
		/// The show output
		/// </summary>
		bool ShowOutput = false;
		/// <summary>
		/// The verify dump
		/// </summary>
		bool VerifyDump = false;

		private MiniDumpFile _miniDumpFile;

		private string _dumpFileName;

		public string DumpFileName { get => _dumpFileName; set => _dumpFileName = value; }
		public MiniDumpFile MiniDumpFile { get => _miniDumpFile; set => _miniDumpFile = value; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DumpCreator"/> class.
		/// </summary>
		/// <param name="showOutput">if set to <c>true</c> [show output].</param>
		/// <param name="verifyDump">if set to <c>true</c> [verify dump].</param>
		public DumpCreator(bool showOutput, bool verifyDump)
		{
			ShowOutput = showOutput;
			VerifyDump = verifyDump;
		}

		public bool Dump(string PID, out string dumpFileName, out int nMemoryRange, out ulong nMemorySize)
		{
			nMemoryRange = 0;
			nMemorySize = 0;

			string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			//suspend process
			string args = String.Format(" {0}", PID);

			ProcessStartInfo psInfo = new ProcessStartInfo(Path.Combine(Path.Combine(appPath, DUMP_FOLDER + @"PsSuspend\"), (Environment.Is64BitProcess ? PsSuspend64 : PsSuspend)), $"{args}")
			{
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};

			var ps = System.Diagnostics.Process.Start(psInfo);
			string line;
			List<string> lines = new List<string>();
			bool procSuspendPass = false;
			while ((line = ps.StandardOutput.ReadLine()) != null)
			{

				if (line.Contains("Process " + PID + " suspended."))
				{
					procSuspendPass = true;
				}
			}

			string tempFolder = Path.Combine(appPath, DUMP_FOLDER + @"temp\");
			if (!File.Exists(tempFolder))
				Directory.CreateDirectory(tempFolder);

			//dump process
			dumpFileName = Path.Combine(tempFolder, Guid.NewGuid().ToString() + DUMP_EXTENSION);
			args = String.Format(" {0} {1} {2}", "-ma", PID, dumpFileName);

			ProcessStartInfo info = new ProcessStartInfo(Path.Combine(Path.Combine(appPath, DUMP_FOLDER + @"ProcDump\"), (Environment.Is64BitProcess ? ProcDumpExe64a : ProcDumpExe)), $"{args}")
			{
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};

			var p = System.Diagnostics.Process.Start(info);
			lines = new List<string>();
			bool procDumpPass = false;
			while ((line = p.StandardOutput.ReadLine()) != null)
			{
				lines.Add(line);
				if (ShowOutput)
				{
					logger.Trace(line);
				}

				if (line.Contains("Error creating dump file"))
				{
					procDumpPass = true;
				}

				if (dumpFileName == null && procDumpPass == false)
				{
					dumpFileName = GetDumpFileName(line);
				}
			}

			if (dumpFileName == null)
			{
				logger.Error($"Error: Could not create dump file with procdump args: {args}!");
				return false;
			}
			else
			{
				logger.Trace($"Dump file {dumpFileName} created.");

				if (VerifyDump && CanLoadDump(dumpFileName, PID, out nMemoryRange, out nMemorySize))
				{
					procDumpPass = true;
				}
			}



			//resume suspended process
			args = String.Format(" -r {0}", PID);

			psInfo = new ProcessStartInfo(Path.Combine(Path.Combine(appPath, DUMP_FOLDER + @"PsSuspend\"), (Environment.Is64BitProcess ? PsSuspend64 : PsSuspend)), $"{args}")
			{
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};

			ps = System.Diagnostics.Process.Start(psInfo);
			lines = new List<string>();
			bool procResumePass = false;
			while ((line = ps.StandardOutput.ReadLine()) != null)
			{
				if (line.Contains("Process " + PID + " resumed."))
				{
					procResumePass = true;
				}
			}


			return (procSuspendPass && procDumpPass && procResumePass);
		}
		

		private bool CanLoadDump(string dumpFileName, string pid, out int nMemoryRange, out ulong nMemorySize)
		{
			nMemoryRange = 0;
			nMemorySize = 0;

			try
			{
				this._dumpFileName = dumpFileName;
				this._miniDumpFile = MiniDumpFile.OpenExisting(dumpFileName);
				
				MiniDumpMemoryDescriptor64[] _memoryList64 = MiniDumpFile.ReadMemory64List().MemoryRanges;
				foreach (MiniDumpMemoryDescriptor64 memoryRange64 in _memoryList64)
				{
					nMemoryRange++;
					nMemorySize += memoryRange64.DataSize;
				}

				MiniDumpMemoryDescriptor[] _memoryList = MiniDumpFile.ReadMemoryList();
				foreach (MiniDumpMemoryDescriptor memoryRange in _memoryList)
				{
					nMemoryRange++;
					nMemorySize += memoryRange.Memory.DataSize;
				}

				return MiniDumpFile is null ? false : true;
			}
			catch
			{
				nMemoryRange = 0;
				nMemorySize = 0;

				return false;
			}
		}

		/// <summary>
		/// Gets the name of the dump file.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		internal string GetDumpFileName(string line)
		{
			string lret = null;
			if (line.Contains(DUMP_EXTENSION))
			{
				lret = line.Substring(line.IndexOf(" ", line.IndexOf("initiated:") + 1) + 1);
			}

			return lret;
		}
	}
}
