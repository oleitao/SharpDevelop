using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.AddIn.Pads.DiagnosisPad
{
	/// <summary>
	/// ProcessCpuConter class
	/// </summary>
	public class ProcessCpuCounter
	{
		/// <summary>
		/// Gets the perf counter for process identifier.
		/// </summary>
		/// <param name="processId">The process identifier.</param>
		/// <param name="processCounterName">Name of the process counter.</param>
		/// <returns></returns>
		public static PerformanceCounter GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
		{
			string instance = GetInstanceNameForProcessId(processId);
			if (string.IsNullOrEmpty(instance))
				return null;

			return new PerformanceCounter("Process", processCounterName, instance);
		}

		/// <summary>
		/// Gets the instance name for process identifier.
		/// </summary>
		/// <param name="processId">The process identifier.</param>
		/// <returns></returns>
		public static string GetInstanceNameForProcessId(int processId)
		{
			var process = System.Diagnostics.Process.GetProcessById(processId);
			string processName = System.IO.Path.GetFileNameWithoutExtension(process.ProcessName);

			PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
			string[] instances = cat.GetInstanceNames()
				.Where(inst => inst.StartsWith(processName))
				.ToArray();

			foreach (string instance in instances)
			{
				using (PerformanceCounter cnt = new PerformanceCounter("Process",
					"ID Process", instance, true))
				{
					int val = (int)cnt.RawValue;
					if (val == processId)
					{
						return instance;
					}
				}
			}
			return null;
		}
	}
}
