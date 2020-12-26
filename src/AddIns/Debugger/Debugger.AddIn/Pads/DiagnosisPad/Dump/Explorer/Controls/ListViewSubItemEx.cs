using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Controls
{
	/// <summary>
	/// ListViewSubItemEx controler
	/// </summary>
	/// <seealso cref="System.Windows.Forms.ListViewItem.ListViewSubItem" />
	internal class ListViewSubItemEx : ListViewItem.ListViewSubItem
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewSubItemEx"/> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <param name="tag">The tag.</param>
		public ListViewSubItemEx(ListViewItem owner, string text, object tag)
             : base(owner, text)
        {
            this.Tag = tag;
        }
    }
}
