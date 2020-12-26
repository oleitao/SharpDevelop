using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.AddIn.Pads.DiagnosisPad
{
	/// <summary>
	/// EventsTracker class
	/// </summary>
	public class EventsTracker
	{
		/// <summary>
		/// The row number
		/// </summary>
		private int _rowNumber;
		/// <summary>
		/// The program name
		/// </summary>
		private string _programName;
		/// <summary>
		/// The thread
		/// </summary>
		private string _thread;
		/// <summary>
		/// The duration
		/// </summary>
		private string _duration;
		/// <summary>
		/// The time
		/// </summary>
		private string _time;
		/// <summary>
		/// Initializes a new instance of the <see cref="EventsTracker"/> class.
		/// </summary>
		public EventsTracker()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventsTracker"/> class.
		/// </summary>
		/// <param name="rowNumber">The row number.</param>
		/// <param name="programName">Name of the program.</param>
		/// <param name="thread">The thread.</param>
		/// <param name="duration">The duration.</param>
		/// <param name="time">The time.</param>
		public EventsTracker(int rowNumber, string programName, string thread, string duration, string time)
		{
			this._rowNumber = rowNumber;
			this._programName = programName;
			this._thread = thread;
			this._duration = duration;
			this._time = time;
		}

		/// <summary>
		/// Gets or sets the row number.
		/// </summary>
		/// <value>
		/// The row number.
		/// </value>
		public int RowNumber { get => _rowNumber; set => _rowNumber = value; }
		/// <summary>
		/// Gets or sets the name of the program.
		/// </summary>
		/// <value>
		/// The name of the program.
		/// </value>
		public string ProgramName { get => _programName; set => _programName = value; }
		/// <summary>
		/// Gets or sets the thread.
		/// </summary>
		/// <value>
		/// The thread.
		/// </value>
		public string Thread { get => _thread; set => _thread = value; }
		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>
		/// The duration.
		/// </value>
		public string Duration { get => _duration; set => _duration = value; }
		/// <summary>
		/// Gets or sets the time.
		/// </summary>
		/// <value>
		/// The time.
		/// </value>
		public string Time { get => _time; set => _time = value; }
	}
}
