using Debugger.MiniDump;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// ThreadNamesView view
	/// </summary>
	/// <seealso cref="System.Windows.Forms.UserControl" />
	public partial class ThreadNamesView : UserControl
    {
		/// <summary>
		/// The thread names stream
		/// </summary>
		private MiniDumpThreadNamesStream _threadNamesStream;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadNamesView"/> class.
		/// </summary>
		public ThreadNamesView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadNamesView"/> class.
		/// </summary>
		/// <param name="threadNamesStream">The thread names stream.</param>
		public ThreadNamesView(MiniDumpThreadNamesStream threadNamesStream)
            : this()
        {
            this._threadNamesStream = threadNamesStream;

            if (_threadNamesStream.Entries.Count == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                foreach (MiniDumpThreadName thread in _threadNamesStream.Entries)
                {
                    ListViewItem newItem = new ListViewItem("0x" + thread.ThreadId.ToString("x8") + " (" + thread.ThreadId + ")");
                    newItem.SubItems.Add(thread.Name);

                    newItem.Tag = thread;

                    this.listView1.Items.Add(newItem);
                }
            }
        }
    }
}
