using Debugger.AddIn.Pads.DiagnosisPad.Dump;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.AddIn.Pads.DiagnosisPad
{
	/// <summary>
	/// MemorySnap class
	/// </summary>
	public class MemorySnap
	{
		/// <summary>
		/// The identifier
		/// </summary>
		private int _id;
		/// <summary>
		/// The time
		/// </summary>
		private string _time;
		/// <summary>
		/// The memoryrange
		/// </summary>
		private string _memoryrange;
		/// <summary>
		/// The memorysize
		/// </summary>
		private string _memorysize;
		/// <summary>
		/// The dump
		/// </summary>
		private DumpCreator _dump;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemorySnap"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="time">The time.</param>
		/// <param name="memoryrange">The memoryrange.</param>
		/// <param name="memorysize">The memorysize.</param>
		/// <param name="dump">The dump.</param>
		public MemorySnap(int id, string time, string memoryrange, string memorysize, DumpCreator dump)
		{
			this._id = id;
			this._time = time;
			this._memoryrange = memoryrange;
			this._memorysize = memorysize;
			this._dump = dump;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public int ID { get => _id; set => _id = value; }
		/// <summary>
		/// Gets or sets the time.
		/// </summary>
		/// <value>
		/// The time.
		/// </value>
		public string Time { get => _time; set => _time = value; }
		/// <summary>
		/// Gets or sets the memory range.
		/// </summary>
		/// <value>
		/// The memory range.
		/// </value>
		public string MemoryRange { get => _memoryrange; set => _memoryrange = value; }
		/// <summary>
		/// Gets or sets the size of the memory.
		/// </summary>
		/// <value>
		/// The size of the memory.
		/// </value>
		public string MemorySize { get => _memorysize; set => _memorysize = value; }
		/// <summary>
		/// Gets or sets the dump.
		/// </summary>
		/// <value>
		/// The dump.
		/// </value>
		public DumpCreator Dump { get => _dump; set => _dump = value; }
	}
}
