using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// UnloadedModulesView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class UnloadedModulesView : BaseViewControl
    {
		/// <summary>
		/// The unloaded modules stream
		/// </summary>
		private MiniDumpUnloadedModulesStream _unloadedModulesStream;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnloadedModulesView"/> class.
		/// </summary>
		public UnloadedModulesView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="UnloadedModulesView"/> class.
		/// </summary>
		/// <param name="unloadedModulesStream">The unloaded modules stream.</param>
		public UnloadedModulesView(MiniDumpUnloadedModulesStream unloadedModulesStream)
            : this()
        {
            _unloadedModulesStream = unloadedModulesStream;

            if (unloadedModulesStream.NumberOfEntries == 0)
            {
                this.listView1.Items.Add("No data found for stream");
                return;
            }

            foreach (MiniDumpUnloadedModule unloadedModule in _unloadedModulesStream.Entries)
            {
                ListViewItem newItem = new ListViewItem(unloadedModule.ModuleName);
                newItem.SubItems.Add(Formatters.FormatAsSizeString(unloadedModule.SizeOfImage));
                newItem.SubItems.Add(unloadedModule.TimeDateStamp.ToString());
                newItem.SubItems.Add(Formatters.FormatAsMemoryAddress(unloadedModule.BaseOfImage));

                newItem.Tag = unloadedModule;

                this.listView1.Items.Add(newItem);
            }
        }
    }
}
