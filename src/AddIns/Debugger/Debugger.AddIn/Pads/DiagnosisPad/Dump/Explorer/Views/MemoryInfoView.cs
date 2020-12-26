using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Debugger.MiniDump;
using Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Utilities;
using Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Dialogs;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// MemoryInfoView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class MemoryInfoView : BaseViewControl
    {
		/// <summary>
		/// The memory information stream
		/// </summary>
		private MiniDumpMemoryInfoStream _memoryInfoStream;
		/// <summary>
		/// The minidump file
		/// </summary>
		private MiniDumpFile _minidumpFile;

		/// <summary>
		/// The col base address
		/// </summary>
		private const int COL_BASE_ADDRESS = 0;
		/// <summary>
		/// The col allocation base
		/// </summary>
		private const int COL_ALLOCATION_BASE = 1;
		/// <summary>
		/// The col allocation protect
		/// </summary>
		private const int COL_ALLOCATION_PROTECT = 2;
		/// <summary>
		/// The col region size
		/// </summary>
		private const int COL_REGION_SIZE = 3;
		/// <summary>
		/// The col state
		/// </summary>
		private const int COL_STATE = 4;
		/// <summary>
		/// The col protect
		/// </summary>
		private const int COL_PROTECT = 5;
		/// <summary>
		/// The col type
		/// </summary>
		private const int COL_TYPE = 6;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryInfoView"/> class.
		/// </summary>
		public MemoryInfoView()
        {
            InitializeComponent();

            listView1.SetFilteringForColumn(COL_ALLOCATION_PROTECT, true);
            listView1.SetFilteringForColumn(COL_STATE, true);
            listView1.SetFilteringForColumn(COL_PROTECT, true);
            listView1.SetFilteringForColumn(COL_TYPE, true);

            listView1.ColumnSorter.SortColumn(COL_BASE_ADDRESS, listItem => (listItem.Tag as MiniDumpMemoryInfo).BaseAddress);
            listView1.ColumnSorter.SortColumn(COL_ALLOCATION_BASE, listItem => (listItem.Tag as MiniDumpMemoryInfo).AllocationBase);
            listView1.ColumnSorter.SortColumn(COL_ALLOCATION_PROTECT, listItem => (listItem.Tag as MiniDumpMemoryInfo).AllocationProtect.ToString());
            listView1.ColumnSorter.SortColumn(COL_REGION_SIZE, listItem => (listItem.Tag as MiniDumpMemoryInfo).RegionSize);
            listView1.ColumnSorter.SortColumn(COL_STATE, listItem => (listItem.Tag as MiniDumpMemoryInfo).State.ToString());
            listView1.ColumnSorter.SortColumn(COL_PROTECT, listItem => (listItem.Tag as MiniDumpMemoryInfo).Protect.ToString());
            listView1.ColumnSorter.SortColumn(COL_TYPE, listItem => (listItem.Tag as MiniDumpMemoryInfo).Type.ToString());
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryInfoView"/> class.
		/// </summary>
		/// <param name="memoryInfoStream">The memory information stream.</param>
		/// <param name="minidumpFile">The minidump file.</param>
		public MemoryInfoView(MiniDumpMemoryInfoStream memoryInfoStream, MiniDumpFile minidumpFile)
            : this()
        {
            _memoryInfoStream = memoryInfoStream;
            _minidumpFile = minidumpFile;

            if (_memoryInfoStream.NumberOfEntries == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                List<ListViewItem> listItems = new List<ListViewItem>();

                foreach (MiniDumpMemoryInfo memoryInfo in _memoryInfoStream.Entries)
                {
                    ListViewItem newItem = new ListViewItem(Formatters.FormatAsMemoryAddress(memoryInfo.BaseAddress));
                    newItem.Tag = memoryInfo;

                    // If the state is MEM_FREE then AllocationProtect, RegionSize, Protect and Type are undefined.
                    if (memoryInfo.State == MemoryState.MEM_FREE)
                    {
                        newItem.SubItems.Add(string.Empty);
                        newItem.SubItems.Add(string.Empty);
                        newItem.SubItems.Add(memoryInfo.RegionSizePretty);
                        newItem.SubItems.Add(memoryInfo.State.ToString());
                        newItem.SubItems.Add(string.Empty);
                        newItem.SubItems.Add(string.Empty);
                    }
                    else
                    {
                        newItem.SubItems.Add(Formatters.FormatAsMemoryAddress(memoryInfo.AllocationBase));
                        newItem.SubItems.Add(memoryInfo.AllocationProtect.ToString());
                        newItem.SubItems.Add(memoryInfo.RegionSizePretty);
                        newItem.SubItems.Add(memoryInfo.State.ToString());
                        // Some regions don't have any Protection information
                        newItem.SubItems.Add(((int) memoryInfo.Protect == 0) ? string.Empty : memoryInfo.Protect.ToString());
                        newItem.SubItems.Add(memoryInfo.Type.ToString());
                    }

                    listItems.Add(newItem);
                }

                listView1.AddItemsForFiltering(listItems);
            }
        }

		#region Event handlers

		/// <summary>
		/// Handles the DoubleClick event of the MemoryInfoView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void MemoryInfoView_DoubleClick(object sender, EventArgs e)
        {
            DisplaySelectedMemoryBlock();
        }

		/// <summary>
		/// Handles the Click event of the viewToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySelectedMemoryBlock();
        }

		/// <summary>
		/// Handles the KeyPress event of the listView1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
		private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                DisplaySelectedMemoryBlock();

                e.Handled = true;
            }
        }

		#endregion

		/// <summary>
		/// Displays the selected memory block.
		/// </summary>
		private void DisplaySelectedMemoryBlock()
        {
            MiniDumpMemoryInfo memoryBlock = (MiniDumpMemoryInfo)listView1.SelectedItems[0].Tag;

            // First check if we have all of the process memory, if we don't then there's no need to proceed.
            if ((this.Memory64Stream == null) || (this.Memory64Stream.MemoryRanges.Length <= 0))
            {
                MessageBox.Show("Memory information is only available when using a full-memory dump.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ulong startAddress = memoryBlock.BaseAddress;
            ulong endAddress = memoryBlock.BaseAddress + memoryBlock.RegionSize - 1;
            ulong offsetToReadFrom = MiniDumpHelper.FindMemory64Block(this.Memory64Stream, startAddress, endAddress);

            if (offsetToReadFrom == 0)
            {
                MessageBox.Show("Sorry, I couldn't locate the data for that memory region inside the minidump.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            byte[] data = new byte[memoryBlock.RegionSize];

            _minidumpFile.CopyMemoryFromOffset(offsetToReadFrom, data, (uint)memoryBlock.RegionSize);

            HexViewerDialog hexViewerDialog = new HexViewerDialog(data);
            hexViewerDialog.Text = $"Displaying {Formatters.FormatAsMemoryAddress(startAddress)} - {Formatters.FormatAsMemoryAddress(endAddress)} ({Formatters.FormatAsSizeString(memoryBlock.RegionSize)}, {memoryBlock.RegionSize} bytes)";

            hexViewerDialog.Show();
        }

		/// <summary>
		/// The memory64 stream
		/// </summary>
		private MiniDumpMemory64Stream _memory64Stream;

		/// <summary>
		/// Gets the memory64 stream.
		/// </summary>
		/// <value>
		/// The memory64 stream.
		/// </value>
		private MiniDumpMemory64Stream Memory64Stream
        {
            get
            {
                if (_memory64Stream == null)
                    _memory64Stream = _minidumpFile.ReadMemory64List();

                return _memory64Stream;
            }
        }

    }
}
