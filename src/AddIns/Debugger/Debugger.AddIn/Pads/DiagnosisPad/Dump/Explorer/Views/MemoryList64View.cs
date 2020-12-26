using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// MemoryList64View view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class MemoryList64View : BaseViewControl
    {
		/// <summary>
		/// The memory list
		/// </summary>
		private MiniDumpMemoryDescriptor64[] _memoryList;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryList64View"/> class.
		/// </summary>
		public MemoryList64View()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryList64View"/> class.
		/// </summary>
		/// <param name="memoryList">The memory list.</param>
		public MemoryList64View(MiniDumpMemoryDescriptor64[] memoryList)
            : this()
        {
            _memoryList = memoryList;

            if (_memoryList.Length == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                foreach (MiniDumpMemoryDescriptor64 memoryRange in memoryList)
                {
                    ListViewItem newItem = new ListViewItem(memoryRange.StartOfMemoryRangeFormatted);
                    newItem.SubItems.Add(memoryRange.EndOfMemoryRangeFormatted);
                    newItem.SubItems.Add(memoryRange.DataSizePretty);

                    this.listView1.Items.Add(newItem);
                }
            }
        }
    }
}
