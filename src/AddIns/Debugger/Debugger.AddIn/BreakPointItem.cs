using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Debugger.AddIn
{
	public class BreakPointItem
	{
		public Breakpoint Breakpoint { get; private set; }
		public uint ID { get; private set; }
		public string Event { get; private set; }
		public string Time { get; private set; }
		public string Duration { get; private set; }
		public string Frozen { get; private set; }

		public BreakPointItem(Breakpoint breakpoint, List<Thread> thread, double duration)
		{
			// We want to egarly evaluate the properties while the process is paused
			// rather then wait until the GUI acesses them at some unspecified point
			this.Breakpoint = breakpoint;
			this.ID = thread.Last().ID;
			this.Event = thread.Last().Name;
			this.Time = duration.ToString();
			this.Duration = duration.ToString();
			this.Frozen = ResourceService.GetString(thread.Last().Suspended ? "Global.Yes" : "Global.No");

			//this.Thread = thread;
			//this.ID = thread.ID;
			//this.Name = thread.Name;
			//this.Priority = PriorityToString(thread.Priority);

		}
	}
}
