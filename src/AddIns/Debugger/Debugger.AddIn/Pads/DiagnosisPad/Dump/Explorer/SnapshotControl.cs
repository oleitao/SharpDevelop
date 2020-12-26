using System;
using System.Windows.Forms;
using System.IO;
using Debugger.MiniDump;
using Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer
{
	/// <summary>
	/// SnapshotControl class get dump info
	/// </summary>
	/// <seealso cref="System.Windows.Forms.UserControl" />
	public partial class SnapshotControl : UserControl
	{
		/// <summary>
		/// The allowed drop extensions
		/// </summary>
		private readonly string[] ALLOWED_DROP_EXTENSIONS = { ".hdmp", ".dmp" };

		/// <summary>
		/// The dump
		/// </summary>
		private DumpCreator _dump;
		/// <summary>
		/// Gets or sets the dump.
		/// </summary>
		/// <value>
		/// The dump.
		/// </value>
		public DumpCreator Dump { get => _dump; set => _dump = value; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SnapshotControl"/> class.
		/// </summary>
		/// <param name="dump">The dump.</param>
		public SnapshotControl(DumpCreator dump)
		{
			InitializeComponent();

			this._dump = dump;

			OpenNewSession();
		}

		/// <summary>
		/// Handles the Load event of the SnapshotControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void SnapshotControl_Load(object sender, EventArgs e)
		{
			treeViewExplorer.ExpandAll();
		}

		/// <summary>
		/// Handles the AfterSelect event of the treeSnapDetails control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="TreeViewEventArgs"/> instance containing the event data.</param>
		private void treeSnapDetails_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (Dump.MiniDumpFile == null)
				return;

			CmdDisplayStream((string)e.Node.Tag);
		}

		/// <summary>
		/// Opens the new session.
		/// </summary>
		private void OpenNewSession()
		{
			ResetTree();
			CloseExistingSession();

			try
			{
				_dump.MiniDumpFile = MiniDumpFile.OpenExisting(_dump.DumpFileName);

				this.treeViewExplorer.Nodes[0].Text = Path.GetFileName(_dump.DumpFileName);
				this.treeViewExplorer.Nodes[0].ToolTipText = _dump.DumpFileName;
				this.treeViewExplorer.SelectedNode = treeViewExplorer.Nodes[0];

				CmdDisplayStream("Summary");
			}
			catch (Exception e)
			{
				MessageBox.Show($"An error occurred while attempting to load your minidump:\r\n\r\n\"{e.Message.TrimEnd(null)}\"", "Error Loading Minidump", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Closes the existing session.
		/// </summary>
		private void CloseExistingSession()
		{
			if (this.splitContainer1.Panel2.Controls.Count > 0) this.splitContainer1.Panel2.Controls.RemoveAt(0);

			if (_dump.MiniDumpFile != null) _dump.MiniDumpFile.Dispose();

			this.treeViewExplorer.Nodes[0].Text = "<No minidump loaded>";
		}

		/// <summary>
		/// Resets the tree.
		/// </summary>
		private void ResetTree()
		{
			// TODO: Not pretty, but it'll do for now.
			foreach (TreeNode node in treeViewExplorer.Nodes[0].Nodes)
			{
				node.Text = (string)node.Tag;
			}
		}

		/// <summary>
		/// Commands the display stream.
		/// </summary>
		/// <param name="streamName">Name of the stream.</param>
		private void CmdDisplayStream(string streamName)
		{
			UserControl viewToDisplay = null;
			int numberOfItems = 0;
			string nodeText = String.Empty; // Quick fix for duplicated item counts in node text

			switch (streamName)
			{
				case "Summary":
					nodeText = string.Empty;
					viewToDisplay = new SummaryView(this._dump.MiniDumpFile);
					break;
				case "Handles":
					nodeText = "Handles";
					MiniDumpHandleDescriptor[] handleData = this._dump.MiniDumpFile.ReadHandleData();
					numberOfItems = handleData.Length;
					viewToDisplay = new HandleDataView(handleData);
					break;
				case "Modules":
					nodeText = "Modules";
					MiniDumpModule[] moduleData = this._dump.MiniDumpFile.ReadModuleList();
					numberOfItems = moduleData.Length;
					viewToDisplay = new ModulesView(moduleData);
					break;
				case "Threads":
					nodeText = "Threads";
					MiniDumpThread[] threadData = this._dump.MiniDumpFile.ReadThreadList();
					numberOfItems = threadData.Length;
					viewToDisplay = new ThreadListView(threadData);
					break;
				case "ThreadInfo":
					nodeText = "ThreadInfo";
					MiniDumpThreadInfo[] threadInfoData = this._dump.MiniDumpFile.ReadThreadInfoList();
					numberOfItems = threadInfoData.Length;
					viewToDisplay = new ThreadInfoListView(threadInfoData);
					break;
				case "ThreadNames":
					nodeText = "ThreadNames";
					MiniDumpThreadNamesStream threadNamesStream = this._dump.MiniDumpFile.ReadThreadNamesStream();
					numberOfItems = threadNamesStream.Entries.Count;
					viewToDisplay = new ThreadNamesView(threadNamesStream);
					break;
				case "Memory":
					nodeText = "Memory";
					MiniDumpMemoryDescriptor[] memoryData = this._dump.MiniDumpFile.ReadMemoryList();
					numberOfItems = memoryData.Length;
					viewToDisplay = new MemoryListView(memoryData);
					break;
				case "Memory64":
					nodeText = "Memory64";
					MiniDumpMemory64Stream memory64Data = this._dump.MiniDumpFile.ReadMemory64List();
					numberOfItems = memory64Data.MemoryRanges.Length;
					viewToDisplay = new MemoryList64View(memory64Data.MemoryRanges);
					break;
				case "MemoryInfo":
					nodeText = "MemoryInfo";
					MiniDumpMemoryInfoStream memoryInfo = this._dump.MiniDumpFile.ReadMemoryInfoList();
					numberOfItems = memoryInfo.Entries.Length;
					viewToDisplay = new MemoryInfoView(memoryInfo, _dump.MiniDumpFile);
					break;
				case "MiscInfo":
					nodeText = "MiscInfo";
					MiniDumpMiscInfo miscInfo = this._dump.MiniDumpFile.ReadMiscInfo();
					numberOfItems = 1;
					viewToDisplay = new MiscInfoView(miscInfo);
					break;
				case "SystemInfo":
					nodeText = "SystemInfo";
					MiniDumpSystemInfoStream systemInfo = this._dump.MiniDumpFile.ReadSystemInfo();
					numberOfItems = 1;
					viewToDisplay = new SystemInfoView(systemInfo);
					break;
				case "Exception":
					nodeText = "Exception";
					MiniDumpExceptionStream exceptionStream = this._dump.MiniDumpFile.ReadExceptionStream();

					numberOfItems = (exceptionStream == null) ? 0 : 1;

					viewToDisplay = new ExceptionStreamView(exceptionStream);
					break;
				case "UnloadedModules":
					nodeText = "UnloadedModules";
					MiniDumpUnloadedModulesStream unloadedModulesStream = this._dump.MiniDumpFile.ReadUnloadedModuleList();
					numberOfItems = (int)unloadedModulesStream.NumberOfEntries;
					viewToDisplay = new UnloadedModulesView(unloadedModulesStream);
					break;
				case "SystemMemoryInfo":
					nodeText = "SystemMemoryInfo";
					MiniDumpSystemMemoryInfo systemMemoryInfo = this._dump.MiniDumpFile.ReadSystemMemoryInfo();
					numberOfItems = 1;
					viewToDisplay = new SystemMemoryInfoView(systemMemoryInfo);
					break;
			}

			if (viewToDisplay != null)
			{
				if (nodeText != string.Empty) treeViewExplorer.SelectedNode.Text = nodeText + " (" + numberOfItems + (numberOfItems == 1 ? " item" : " items") + ")";

				if (this.splitContainer1.Panel2.Controls.Count > 0) this.splitContainer1.Panel2.Controls.RemoveAt(0);

				viewToDisplay.Dock = DockStyle.Fill;

				this.splitContainer1.Panel2.Controls.Add(viewToDisplay);
			}
		}
	}
}
