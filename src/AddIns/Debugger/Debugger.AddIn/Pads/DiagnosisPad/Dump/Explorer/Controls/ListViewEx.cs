using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Controls
{
	// Adapted from http://stackoverflow.com/questions/7705381/adding-filter-boxes-to-the-column-headers-of-a-listview-in-c-sharp-and-winforms
	/// <summary>
	/// ListViewEx controler view
	/// </summary>
	/// <seealso cref="System.Windows.Forms.ListView" />
	class ListViewEx : ListView
    {
		/// <summary>
		/// 
		/// </summary>
		public enum SortImage
        {
			/// <summary>
			/// The none
			/// </summary>
			None = 0x0,
			/// <summary>
			/// The descending
			/// </summary>
			Descending = 0x200,
			/// <summary>
			/// The ascending
			/// </summary>
			Ascending = 0x400
        }

		/// <summary>
		/// The header dropdowns
		/// </summary>
		private List<bool> HeaderDropdowns = new List<bool>();

		/// <summary>
		/// 
		/// </summary>
		/// <seealso cref="System.EventArgs" />
		public class HeaderDropdownArgs : EventArgs
        {
			/// <summary>
			/// Gets or sets the column.
			/// </summary>
			/// <value>
			/// The column.
			/// </value>
			public int Column { get; set; }
			/// <summary>
			/// Gets or sets the control.
			/// </summary>
			/// <value>
			/// The control.
			/// </value>
			public Control Control { get; set; }
        }
		/// <summary>
		/// Occurs when [header dropdown].
		/// </summary>
		public event EventHandler<HeaderDropdownArgs> HeaderDropdown;

		/// <summary>
		/// Displays the sort image on column.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="image">The image.</param>
		public void DisplaySortImageOnColumn(int column, SortImage image)
        {
            HDITEM.Format format = GetColumnFormat(column);

            // Reset any existing sort images
            format &= ~HDITEM.Format.SortDown;
            format &= ~HDITEM.Format.SortUp;

            // Set new image (if any)
            if (image == SortImage.Ascending)
                format |= HDITEM.Format.SortUp;
            else if (image == SortImage.Descending)
                format |= HDITEM.Format.SortDown;

            SetColumnFormat(column, format);
        }

		/// <summary>
		/// Sets the header dropdown.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="enable">if set to <c>true</c> [enable].</param>
		/// <exception cref="ArgumentOutOfRangeException">column</exception>
		protected void SetHeaderDropdown(int column, bool enable)
        {
            if (column < 0 || column >= this.Columns.Count)
                throw new ArgumentOutOfRangeException(nameof(column));

            while (HeaderDropdowns.Count < this.Columns.Count)
                HeaderDropdowns.Add(false);

            HeaderDropdowns[column] = enable;

            if (this.IsHandleCreated)
                EnableSplitButtonOnColumnHeader(column, enable);
        }

		/// <summary>
		/// Called when [header dropdown].
		/// </summary>
		/// <param name="column">The column.</param>
		protected void OnHeaderDropdown(int column)
        {
            var handler = HeaderDropdown;
            if (handler == null) return;
            var args = new HeaderDropdownArgs() { Column = column };
            handler(this, args);
            if (args.Control == null) return;
            //var frm = new Form();
            //frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            //frm.ShowInTaskbar = false;
            //frm.ControlBox = false;
            //args.Control.Location = Point.Empty;
            //frm.Controls.Add(args.Control);
            //frm.Load += delegate { frm.MinimumSize = new Size(1, 1); frm.Size = frm.Controls[0].Size; };
            //frm.Deactivate += delegate { frm.Dispose(); };
            //frm.StartPosition = FormStartPosition.Manual;
            //var rc = GetHeaderRect(column);
            //frm.Location = this.PointToScreen(new Point(rc.Right - SystemInformation.MenuButtonSize.Width, rc.Bottom));
            //frm.Show(this.FindForm());
        }

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.Columns.Count == 0 || Environment.OSVersion.Version.Major < 6 || HeaderDropdowns == null) return;
            for (int col = 0; col < HeaderDropdowns.Count; ++col)
            {
                if (HeaderDropdowns[col]) EnableSplitButtonOnColumnHeader(col, true);
            }
        }

		/// <summary>
		/// Gets the header rect.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		protected Rectangle GetHeaderRect(int column)
        {
            IntPtr hHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            RECT rc;
            SendMessage(hHeader, HDM_GETITEMRECT, (IntPtr)column, out rc);
            return new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }

		/// <summary>
		/// Gets the split button rect.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		protected Rectangle GetSplitButtonRect(int column)
        {
            IntPtr hHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            RECT rc;
            SendMessage(hHeader, HDM_GETITEMDROPDOWNRECT, (IntPtr)column, out rc);
            return new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }

		/// <summary>
		/// Enables the split button on column header.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="enable">if set to <c>true</c> [enable].</param>
		protected void EnableSplitButtonOnColumnHeader(int column, bool enable)
        {
            HDITEM headerItem = new HDITEM();
            headerItem.mask = HDITEM.Mask.Format;

            IntPtr columnHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            SendMessage(columnHeader, HDM_GETITEM, (IntPtr)column, ref headerItem);

            if (enable)
                headerItem.fmt |= HDITEM.Format.SplitButton;
            else
                headerItem.fmt &= ~HDITEM.Format.SplitButton;

            IntPtr res = SendMessage(columnHeader, HDM_SETITEM, (IntPtr)column, ref headerItem);
        }

		/// <summary>
		/// Gets the column format.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		/// <exception cref="Win32Exception"></exception>
		private HDITEM.Format GetColumnFormat(int column)
        {
            IntPtr hresult;
            IntPtr columnHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            HDITEM item = new HDITEM();
            item.mask = HDITEM.Mask.Format;

            hresult = SendMessage(columnHeader, HDM_GETITEM, (IntPtr)column, ref item);
            if (hresult != TRUE)
                throw new Win32Exception();
            else
                return item.fmt;
        }

		/// <summary>
		/// Sets the column format.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="format">The format.</param>
		private void SetColumnFormat(int column, HDITEM.Format format)
        {
            IntPtr hresult;
            IntPtr columnHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            HDITEM item = new HDITEM();
            item.mask = HDITEM.Mask.Format;
            item.fmt = format;

            //hresult = SendMessage(columnHeader, HDM_GETITEM, (IntPtr) column, ref item);
            //if (hresult != TRUE) return;

            //item.fmt |= format;
            hresult = SendMessage(columnHeader, HDM_SETITEM, (IntPtr)column, ref item);
        }

		/// <summary>
		/// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
		protected override void WndProc(ref Message m)
        {
            //Console.WriteLine(m);
            if (m.Msg == WM_NOTIFY)
            {
                var hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                if (hdr.code == LVN_COLUMNDROPDOWN)
                {
                    var info = (NMLISTVIEW)Marshal.PtrToStructure(m.LParam, typeof(NMLISTVIEW));
                    OnHeaderDropdown(info.iSubItem);
                    return;
                }
            }
            base.WndProc(ref m);
        }

		// Pinvoke
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="wp">The wp.</param>
		/// <param name="lp">The lp.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="wp">The wp.</param>
		/// <param name="lvc">The LVC.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, ref LVCOLUMN lvc);
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="wParam">The w parameter.</param>
		/// <param name="lParam">The l parameter.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref HDITEM lParam);
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="wp">The wp.</param>
		/// <param name="rc">The rc.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, out RECT rc);
		/// <summary>
		/// Sets the parent.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="hParent">The h parent.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hParent);

		/// <summary>
		/// The true
		/// </summary>
		private readonly IntPtr TRUE = new IntPtr(1);
		/// <summary>
		/// The false
		/// </summary>
		private readonly IntPtr FALSE = new IntPtr(0);
		/// <summary>
		/// The LVM first
		/// </summary>
		private const int LVM_FIRST = 0x1000;
		/// <summary>
		/// The LVM getcolumn
		/// </summary>
		private const int LVM_GETCOLUMN = LVM_FIRST + 95;
		/// <summary>
		/// The LVM setcolumn
		/// </summary>
		private const int LVM_SETCOLUMN = LVM_FIRST + 96;
		/// <summary>
		/// The LVCF FMT
		/// </summary>
		private const int LVCF_FMT = 1;
		/// <summary>
		/// The LVCFMT left
		/// </summary>
		private const int LVCFMT_LEFT = 0x0000;
		/// <summary>
		/// The LVCF image
		/// </summary>
		private const int LVCF_IMAGE = 0x0010;
		/// <summary>
		/// The LVCFMT image
		/// </summary>
		private const int LVCFMT_IMAGE = 2048; // 0x800
		/// <summary>
		/// The LVCFMT splitbutton
		/// </summary>
		private const int LVCFMT_SPLITBUTTON = 0x1000000;
		/// <summary>
		/// The LVCFMT col has images
		/// </summary>
		private const int LVCFMT_COL_HAS_IMAGES = 0x8000;
		/// <summary>
		/// The wm notify
		/// </summary>
		private const int WM_NOTIFY = 0x204e;
		/// <summary>
		/// The LVN columndropdown
		/// </summary>
		private const int LVN_COLUMNDROPDOWN = -100 - 64;
		/// <summary>
		/// The LVM getheader
		/// </summary>
		private const int LVM_GETHEADER = 0x1000 + 31;
		/// <summary>
		/// The hdi format
		/// </summary>
		private const int HDI_FORMAT = 0x0004;
		/// <summary>
		/// The hdi image
		/// </summary>
		private const int HDI_IMAGE = 0x0020;
		/// <summary>
		/// The hdi order
		/// </summary>
		private const int HDI_ORDER = 0x0080;
		/// <summary>
		/// The HDM first
		/// </summary>
		private const int HDM_FIRST = 0x1200;
		/// <summary>
		/// The HDM getitemrect
		/// </summary>
		private const int HDM_GETITEMRECT = HDM_FIRST + 7;
		/// <summary>
		/// The HDM setimagelist
		/// </summary>
		private const int HDM_SETIMAGELIST = HDM_FIRST + 8;
		/// <summary>
		/// The HDM getitem
		/// </summary>
		private const int HDM_GETITEM = HDM_FIRST + 11;
		/// <summary>
		/// The HDM setitem
		/// </summary>
		private const int HDM_SETITEM = HDM_FIRST + 12;
		/// <summary>
		/// The HDM getitemdropdownrect
		/// </summary>
		private const int HDM_GETITEMDROPDOWNRECT = HDM_FIRST + 25;
		/// <summary>
		/// The HDF splitbutton
		/// </summary>
		private const int HDF_SPLITBUTTON = 0x1000000;
		/// <summary>
		/// The bcsif glyph
		/// </summary>
		private const int BCSIF_GLYPH = 0x0001;
		/// <summary>
		/// The bcsif image
		/// </summary>
		private const int BCSIF_IMAGE = 0x0002;
		/// <summary>
		/// The bcsif style
		/// </summary>
		private const int BCSIF_STYLE = 0x0004;
		/// <summary>
		/// The bcsif size
		/// </summary>
		private const int BCSIF_SIZE = 0x0008;
		/// <summary>
		/// The hdsil normal
		/// </summary>
		private const int HDSIL_NORMAL = 0;
		/// <summary>
		/// The hdsil state
		/// </summary>
		private const int HDSIL_STATE = 1;
		/// <summary>
		/// The i imagenone
		/// </summary>
		private const int I_IMAGENONE = -2;

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LVCOLUMN
        {
			/// <summary>
			/// The mask
			/// </summary>
			public uint mask;
			/// <summary>
			/// The FMT
			/// </summary>
			public int fmt;
			/// <summary>
			/// The cx
			/// </summary>
			public int cx;
			/// <summary>
			/// The PSZ text
			/// </summary>
			public string pszText;
			/// <summary>
			/// The CCH text maximum
			/// </summary>
			public int cchTextMax;
			/// <summary>
			/// The i sub item
			/// </summary>
			public int iSubItem;
			/// <summary>
			/// The i image
			/// </summary>
			public int iImage;
			/// <summary>
			/// The i order
			/// </summary>
			public int iOrder;
			/// <summary>
			/// The cx minimum
			/// </summary>
			public int cxMin;
			/// <summary>
			/// The cx default
			/// </summary>
			public int cxDefault;
			/// <summary>
			/// The cx ideal
			/// </summary>
			public int cxIdeal;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct POINT
        {
			/// <summary>
			/// The x
			/// </summary>
			public int x, y;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RECT
        {
			/// <summary>
			/// The left
			/// </summary>
			public int left, top, right, bottom;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NMHDR
        {
			/// <summary>
			/// The HWND from
			/// </summary>
			public IntPtr hwndFrom;
			/// <summary>
			/// The identifier from
			/// </summary>
			public IntPtr idFrom;
			/// <summary>
			/// The code
			/// </summary>
			public int code;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NMLISTVIEW
        {
			/// <summary>
			/// The HDR
			/// </summary>
			public NMHDR hdr;
			/// <summary>
			/// The i item
			/// </summary>
			public int iItem;
			/// <summary>
			/// The i sub item
			/// </summary>
			public int iSubItem;
			/// <summary>
			/// The u new state
			/// </summary>
			public uint uNewState;
			/// <summary>
			/// The u old state
			/// </summary>
			public uint uOldState;
			/// <summary>
			/// The u changed
			/// </summary>
			public uint uChanged;
			/// <summary>
			/// The pt action
			/// </summary>
			public POINT ptAction;
			/// <summary>
			/// The l parameter
			/// </summary>
			public IntPtr lParam;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
        public struct HDITEM
        {
			/// <summary>
			/// The mask
			/// </summary>
			public Mask mask;
			/// <summary>
			/// The cxy
			/// </summary>
			public int cxy;
			/// <summary>
			/// The PSZ text
			/// </summary>
			[MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
			/// <summary>
			/// The HBM
			/// </summary>
			public IntPtr hbm;
			/// <summary>
			/// The CCH text maximum
			/// </summary>
			public int cchTextMax;
			/// <summary>
			/// The FMT
			/// </summary>
			public Format fmt;
			/// <summary>
			/// The l parameter
			/// </summary>
			public IntPtr lParam;
			// _WIN32_IE >= 0x0300 
			/// <summary>
			/// The i image
			/// </summary>
			public int iImage;
			/// <summary>
			/// The i order
			/// </summary>
			public int iOrder;
			// _WIN32_IE >= 0x0500
			/// <summary>
			/// The type
			/// </summary>
			public uint type;
			/// <summary>
			/// The pv filter
			/// </summary>
			public IntPtr pvFilter;
			// _WIN32_WINNT >= 0x0600
			/// <summary>
			/// The state
			/// </summary>
			public uint state;

			/// <summary>
			/// 
			/// </summary>
			[Flags]
            public enum Mask
            {
				/// <summary>
				/// The format
				/// </summary>
				Format = 0x4,       // HDI_FORMAT
				/// <summary>
				/// The image
				/// </summary>
				Image = 0x0020,     // HDI_IMAGE
				/// <summary>
				/// The order
				/// </summary>
				Order = 0x0080  // HDI_ORDER 
            };

			/// <summary>
			/// 
			/// </summary>
			[Flags]
            public enum Format
            {
				/// <summary>
				/// The left
				/// </summary>
				Left = 0x0000,          // HDF_LEFT
				/// <summary>
				/// The right
				/// </summary>
				Right = 0x0001,         // HDF_RIGHT
				/// <summary>
				/// The center
				/// </summary>
				Center = 0x0002,        // HDF_CENTER
				/// <summary>
				/// The justify mask
				/// </summary>
				JustifyMask = 0x0003,   // HDF_JUSTIFYMASK
				/// <summary>
				/// The RTL reading
				/// </summary>
				RtlReading = 0x0004,    // HDF_RTLREADING
				/// <summary>
				/// The bitmap
				/// </summary>
				Bitmap = 0x2000,        // HDF_BITMAP
				/// <summary>
				/// The string
				/// </summary>
				String = 0x4000,        // HDF_STRING
				/// <summary>
				/// The ownder draw
				/// </summary>
				OwnderDraw = 0x8000,    // HDF_OWNERDRAW
				/// <summary>
				/// The image
				/// </summary>
				Image = 0x0800,         // HDF_IMAGE
				/// <summary>
				/// The bitmap on right
				/// </summary>
				BitmapOnRight = 0x1000, // HDF_BITMAP_ON_RIGHT
				/// <summary>
				/// The sort down
				/// </summary>
				SortDown = 0x200,       // HDF_SORTDOWN
				/// <summary>
				/// The sort up
				/// </summary>
				SortUp = 0x400,         // HDF_SORTUP
				/// <summary>
				/// The checkbox
				/// </summary>
				Checkbox = 0x0040,      // HDF_CHECKBOX
				/// <summary>
				/// The checked
				/// </summary>
				Checked = 0x0080,       // HDF_CHECKED
				/// <summary>
				/// The fixed width
				/// </summary>
				FixedWidth = 0x0100,    // HDF_FIXEDWIDTH
				/// <summary>
				/// The split button
				/// </summary>
				SplitButton = 0x1000000 // HDF_SPLITBUTTON
            };
        };

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
			/// <summary>
			/// The cx
			/// </summary>
			public int cx;
			/// <summary>
			/// The cy
			/// </summary>
			public int cy;
        }

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
        public struct BUTTON_SPLITINFO
        {
			/// <summary>
			/// The mask
			/// </summary>
			public uint mask;
			/// <summary>
			/// The himl glyph
			/// </summary>
			public IntPtr himlGlyph; // HIMAGELIST
			/// <summary>
			/// The u split style
			/// </summary>
			public uint uSplitStyle;
			/// <summary>
			/// The size
			/// </summary>
			public SIZE size;
        }
    }
}
