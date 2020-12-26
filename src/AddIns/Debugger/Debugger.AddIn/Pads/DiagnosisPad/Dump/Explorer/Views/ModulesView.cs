using System;
using System.Windows.Forms;
using Debugger.MiniDump;
using Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Dialogs;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// ModulesView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class ModulesView : BaseViewControl
    {
		/// <summary>
		/// The modules list
		/// </summary>
		private MiniDumpModule[] _modulesList;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModulesView"/> class.
		/// </summary>
		public ModulesView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModulesView"/> class.
		/// </summary>
		/// <param name="modulesList">The modules list.</param>
		public ModulesView(MiniDumpModule[] modulesList)
            : this()
        {
            _modulesList = modulesList;

            foreach (MiniDumpModule loadedModule in _modulesList)
            {
                ListViewItem newItem = new ListViewItem(loadedModule.PathAndFileName);
                newItem.SubItems.Add(loadedModule.SizeOfImageFormatted);
                newItem.SubItems.Add(loadedModule.TimeDateStamp.ToString());
                newItem.SubItems.Add(loadedModule.FileVersion);
                newItem.SubItems.Add(loadedModule.ProductVersion);
                newItem.SubItems.Add(loadedModule.BaseOfImageFormatted);

                newItem.Tag = loadedModule;

                this.listView1.Items.Add(newItem);
            }
        }

		/// <summary>
		/// Handles the DoubleClick event of the listView1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ModuleDetailsDialog detailDialog = new ModuleDetailsDialog((MiniDumpModule)this.listView1.SelectedItems[0].Tag);

            detailDialog.ShowDialog();
        }
    }
}
