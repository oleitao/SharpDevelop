using Debugger.MiniDump;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// ThreadListView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class ThreadListView : BaseViewControl
    {
		/// <summary>
		/// The thread list
		/// </summary>
		private MiniDumpThread[] _threadList;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadListView"/> class.
		/// </summary>
		public ThreadListView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadListView"/> class.
		/// </summary>
		/// <param name="threadList">The thread list.</param>
		public ThreadListView(MiniDumpThread[] threadList)
            : this()
        {
            _threadList = threadList;

            if (_threadList.Length == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                foreach (MiniDumpThread thread in threadList)
                {
                    ListViewItem newItem = new ListViewItem("0x" + thread.ThreadId.ToString("x8") + " (" + thread.ThreadId + ")");
                    newItem.SubItems.Add(thread.SuspendCount.ToString());
                    newItem.SubItems.Add(thread.PriorityClass.ToString());
                    newItem.SubItems.Add(thread.Priority.ToString());
                    newItem.SubItems.Add("0x" + thread.Teb.ToString("x8"));
                    newItem.SubItems.Add(thread.Stack.StartOfMemoryRangeFormatted + " (" + thread.Stack.Memory.DataSizePretty + ")");
                    newItem.SubItems.Add(thread.ThreadContext.DataSize + " bytes");

                    newItem.Tag = thread;

                    this.listView1.Items.Add(newItem);
                }
            }
        }
    }
}
