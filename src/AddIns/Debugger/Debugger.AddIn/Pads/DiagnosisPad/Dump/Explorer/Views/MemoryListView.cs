using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// MemoryListView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class MemoryListView : BaseViewControl
    {
		/// <summary>
		/// The memory list
		/// </summary>
		private MiniDumpMemoryDescriptor[] _memoryList;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryListView"/> class.
		/// </summary>
		public MemoryListView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryListView"/> class.
		/// </summary>
		/// <param name="memoryList">The memory list.</param>
		public MemoryListView(MiniDumpMemoryDescriptor[] memoryList)
            : this()
        {
            _memoryList = memoryList;

            if (_memoryList.Length == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                foreach (MiniDumpMemoryDescriptor memoryRange in memoryList)
                {
                    ListViewItem newItem = new ListViewItem(memoryRange.StartOfMemoryRangeFormatted);
                    newItem.SubItems.Add(memoryRange.EndOfMemoryRangeFormatted);
                    newItem.SubItems.Add(memoryRange.Memory.DataSizePretty);

                    this.listView1.Items.Add(newItem);
                }
            }
        }
    }
}
