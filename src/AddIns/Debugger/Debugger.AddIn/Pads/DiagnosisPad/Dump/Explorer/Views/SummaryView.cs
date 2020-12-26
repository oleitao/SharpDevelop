using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// SummaryView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class SummaryView : BaseViewControl
    {
		/// <summary>
		/// The mini dump file
		/// </summary>
		private MiniDumpFile _miniDumpFile;

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryView"/> class.
		/// </summary>
		public SummaryView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryView"/> class.
		/// </summary>
		/// <param name="miniDumpFile">The mini dump file.</param>
		public SummaryView(MiniDumpFile miniDumpFile)
            : this()
        {
            _miniDumpFile = miniDumpFile;

            #region Header data

            MiniDumpHeader header = _miniDumpFile.ReadHeader();

            if (header == null) return;

            txtDateTime.Text = header.TimeDateStamp.ToString();
            lblAvailableStreamsHeading.Text = $"Available Streams ({header.DirectoryEntries.Count} items)";

            foreach (MiniDumpDirectory directoryEntry in header.DirectoryEntries.OrderBy(entry => entry.StreamType.ToString()))
            {
                ListViewItem newItem = new ListViewItem(directoryEntry.StreamType.ToString());
                newItem.Tag = directoryEntry;
                newItem.SubItems.Add(Formatters.FormatAsMemoryAddress(directoryEntry.Location.Rva));
                newItem.SubItems.Add(directoryEntry.Location.DataSizePretty);

                listView1.Items.Add(newItem);
            }
            #endregion

            #region Module stream
            MiniDumpModule[] modules = _miniDumpFile.ReadModuleList();

            if (modules.Length > 1)
            {
                this.txtMainModule.Text = modules[0].PathAndFileName;
            }
            #endregion

            #region SystemInfo stream
            MiniDumpSystemInfoStream systemInfoStream = _miniDumpFile.ReadSystemInfo();

            if (systemInfoStream != null)
            {
                this.txtOperatingSystem.Text = systemInfoStream.OperatingSystemDescription;

                if (!string.IsNullOrEmpty(systemInfoStream.CSDVersion))
                    this.txtOperatingSystem.Text += $" ({systemInfoStream.CSDVersion})";
            }
            #endregion
        }

		/// <summary>
		/// Handles the Paint event of the lblSeperator2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
		private void lblSeperator2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, lblSeperator2.DisplayRectangle, Color.White, ButtonBorderStyle.Solid);
        }
    }
}
