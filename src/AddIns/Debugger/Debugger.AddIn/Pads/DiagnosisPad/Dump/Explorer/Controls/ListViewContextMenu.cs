using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Controls
{
	/// <summary>
	/// ListViewContextMenu menu control
	/// </summary>
	/// <seealso cref="System.Windows.Forms.ContextMenuStrip" />
	public class ListViewContextMenu : ContextMenuStrip
    {
		/// <summary>
		/// The copy menu item
		/// </summary>
		ToolStripMenuItem _copyMenuItem;
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewContextMenu"/> class.
		/// </summary>
		/// <param name="container">A component that implements <see cref="T:System.ComponentModel.IContainer" /> that is the container of the <see cref="T:System.Windows.Forms.ContextMenuStrip" />.</param>
		public ListViewContextMenu(System.ComponentModel.IContainer container)
            : base(container)
        {
            _copyMenuItem = new ToolStripMenuItem("Copy");
            _copyMenuItem.Name = "Reserved_Copy";

            this.Items.Add(_copyMenuItem);

            this.Opening += ListViewContextMenu_Opening;
        }

		/// <summary>
		/// Handles the Opening event of the ListViewContextMenu control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		void ListViewContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _copyMenuItem.DropDownItems.Clear();

            if (this.SourceControl is ListView)
            {
                ListView parent = (ListView)this.SourceControl;

                foreach (ColumnHeader columnHeader in parent.Columns)
                {
                    ToolStripMenuItem headingMenuItem = new ToolStripMenuItem(columnHeader.Text);
                    headingMenuItem.Tag = parent;
                    headingMenuItem.Click += copyMenuItem_Click;

                    _copyMenuItem.DropDownItems.Add(headingMenuItem);
                }
            }
        }

		/// <summary>
		/// Handles the Click event of the copyMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		void copyMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            ListView parent = (ListView)menuItem.Tag;
            int columnIndex = -1;

            for (int i = 0; i < parent.Columns.Count; i++)
            {
                if (parent.Columns[i].Text == menuItem.Text)
                {
                    columnIndex = i;
                    continue;
                }
            }

            if (columnIndex == -1) return;
            if (parent.SelectedItems.Count != 1) return;

            if (columnIndex < parent.SelectedItems[0].SubItems.Count)
                Clipboard.SetText(parent.SelectedItems[0].SubItems[columnIndex].Text);
            else
                Clipboard.Clear();
        }
    }
}