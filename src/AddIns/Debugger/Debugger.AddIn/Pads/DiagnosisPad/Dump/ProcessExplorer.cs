using System;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump
{
	/// <summary>
	/// ProcessExplorer class process info
	/// </summary>
	/// <seealso cref="System.IEquatable{Debugger.AddIn.Pads.DiagnosisPad.Dump.ProcessExplorer}" />
	public class ProcessExplorer : IEquatable<ProcessExplorer>
	{
		/// <summary>
		/// The virtual memory
		/// </summary>
		private long virtualMemory;
		/// <summary>
		/// The peak virtual memory
		/// </summary>
		private long peakVirtualMemory;
		/// <summary>
		/// The paged memory
		/// </summary>
		private long pagedMemory;
		/// <summary>
		/// The peak paged memory
		/// </summary>
		private long peakPagedMemory;
		/// <summary>
		/// The peak working set
		/// </summary>
		private long peakWorkingSet;
		/// <summary>
		/// The working set
		/// </summary>
		private long workingSet;
		/// <summary>
		/// The private memory
		/// </summary>
		private long privateMemory;
		/// <summary>
		/// The handle count
		/// </summary>
		private long handleCount;
		/// <summary>
		/// The total processor time
		/// </summary>
		private TimeSpan totalProcessorTime;
		/// <summary>
		/// The start time
		/// </summary>
		private DateTime startTime;
		/// <summary>
		/// The user processor time
		/// </summary>
		private TimeSpan userProcessorTime;
		/// <summary>
		/// The total memory
		/// </summary>
		private long totalMemory;

		/// <summary>
		/// Gets the process.
		/// </summary>
		/// <value>
		/// The process.
		/// </value>
		public System.Diagnostics.Process Process { get; }
		/// <summary>
		/// Gets or sets the virtual memory.
		/// </summary>
		/// <value>
		/// The virtual memory.
		/// </value>
		public long VirtualMemory { get => virtualMemory; set => virtualMemory = value; }
		/// <summary>
		/// Gets or sets the peak virtual memory.
		/// </summary>
		/// <value>
		/// The peak virtual memory.
		/// </value>
		public long PeakVirtualMemory { get => peakVirtualMemory; set => peakVirtualMemory = value; }
		/// <summary>
		/// Gets or sets the paged memory.
		/// </summary>
		/// <value>
		/// The paged memory.
		/// </value>
		public long PagedMemory { get => pagedMemory; set => pagedMemory = value; }
		/// <summary>
		/// Gets or sets the peak paged memory.
		/// </summary>
		/// <value>
		/// The peak paged memory.
		/// </value>
		public long PeakPagedMemory { get => peakPagedMemory; set => peakPagedMemory = value; }
		/// <summary>
		/// Gets or sets the peak working set.
		/// </summary>
		/// <value>
		/// The peak working set.
		/// </value>
		public long PeakWorkingSet { get => peakWorkingSet; set => peakWorkingSet = value; }
		/// <summary>
		/// Gets or sets the working set.
		/// </summary>
		/// <value>
		/// The working set.
		/// </value>
		public long WorkingSet { get => workingSet; set => workingSet = value; }
		/// <summary>
		/// Gets or sets the private memory.
		/// </summary>
		/// <value>
		/// The private memory.
		/// </value>
		public long PrivateMemory { get => privateMemory; set => privateMemory = value; }
		/// <summary>
		/// Gets or sets the handle count.
		/// </summary>
		/// <value>
		/// The handle count.
		/// </value>
		public long HandleCount { get => handleCount; set => handleCount = value; }
		/// <summary>
		/// Gets or sets the total processor time.
		/// </summary>
		/// <value>
		/// The total processor time.
		/// </value>
		public TimeSpan TotalProcessorTime { get => totalProcessorTime; set => totalProcessorTime = value; }
		/// <summary>
		/// Gets or sets the start time.
		/// </summary>
		/// <value>
		/// The start time.
		/// </value>
		public DateTime StartTime { get => startTime; set => startTime = value; }
		/// <summary>
		/// Gets or sets the user processor time.
		/// </summary>
		/// <value>
		/// The user processor time.
		/// </value>
		public TimeSpan UserProcessorTime { get => userProcessorTime; set => userProcessorTime = value; }
		/// <summary>
		/// Gets or sets the total memory.
		/// </summary>
		/// <value>
		/// The total memory.
		/// </value>
		public long TotalMemory { get => totalMemory; set => totalMemory = value; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessExplorer"/> class.
		/// </summary>
		/// <param name="process">The process.</param>
		public ProcessExplorer(System.Diagnostics.Process process)
		{
			Process = process;

			this.virtualMemory = Process.VirtualMemorySize64;
			this.peakVirtualMemory = Process.PeakVirtualMemorySize64;
			this.pagedMemory = Process.PagedMemorySize64;
			this.PeakPagedMemory = Process.PeakPagedMemorySize64;
			this.PeakWorkingSet = Process.PeakWorkingSet64;
			this.WorkingSet = Process.WorkingSet64;
			this.PrivateMemory = Process.PrivateMemorySize64;
			this.HandleCount = Process.HandleCount;
			this.TotalProcessorTime = Process.TotalProcessorTime;
			this.StartTime = Process.StartTime;
			this.UserProcessorTime = Process.UserProcessorTime;
			this.TotalMemory = GC.GetTotalMemory(false);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
		/// </returns>
		public bool Equals(ProcessExplorer other)
		{
			return other.Process.Id == Process.Id;
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("[{1}] {0}", Process.ProcessName, Process.Id);
		}
	}
}