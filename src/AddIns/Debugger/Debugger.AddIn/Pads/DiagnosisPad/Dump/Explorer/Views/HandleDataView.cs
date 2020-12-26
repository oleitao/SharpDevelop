using System.Collections.Generic;
using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// HandleDataView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class HandleDataView : BaseViewControl
    {
		/// <summary>
		/// The col identifier
		/// </summary>
		private const int COL_ID = 0;
		/// <summary>
		/// The col handle type
		/// </summary>
		private const int COL_HANDLE_TYPE = 1;
		/// <summary>
		/// The col object
		/// </summary>
		private const int COL_OBJECT = 2;

		/// <summary>
		/// Initializes a new instance of the <see cref="HandleDataView"/> class.
		/// </summary>
		public HandleDataView()
        {
            InitializeComponent();

            listView1.SetFilteringForColumn(COL_HANDLE_TYPE, true);

            listView1.ColumnSorter.SortColumn(COL_ID, listViewItem => (listViewItem.Tag as MiniDumpHandleDescriptor).HandleId);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="HandleDataView"/> class.
		/// </summary>
		/// <param name="handles">The handles.</param>
		public HandleDataView(MiniDumpHandleDescriptor[] handles)
            : this()
        {
            List<ListViewItem> listItems = new List<ListViewItem>();

            foreach (MiniDumpHandleDescriptor handle in handles)
            {
                ListViewItem newItem = new ListViewItem(Formatters.FormatAsHex(handle.HandleId));
                newItem.Tag = handle;

                newItem.SubItems.Add(handle.TypeName);
                newItem.SubItems.Add(handle.ObjectName);

                listItems.Add(newItem);
            }

            listView1.AddItemsForFiltering(listItems);
        }
    }
}
