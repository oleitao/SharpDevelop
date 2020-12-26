namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer
{
	partial class SnapshotControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				if (_dump.MiniDumpFile != null)
					_dump.MiniDumpFile.Dispose();

				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Exception", 6, 6);
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Handles", 4, 4);
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Memory", 3, 3);
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Memory64", 3, 3);
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("MemoryInfo", 3, 3);
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("MiscInfo", 5, 5);
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Modules", 2, 2);
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("SystemInfo");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("SystemMemoryInfo", 3, 3);
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Threads", 1, 1);
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("ThreadInfo", 1, 1);
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("ThreadNames", 1, 1);
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("UnloadedModules", 2, 2);
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("<No minidump loaded>", new System.Windows.Forms.TreeNode[] {
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshotControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewExplorer = new System.Windows.Forms.TreeView();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewExplorer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Size = new System.Drawing.Size(1117, 480);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewExplorer
            // 
            this.treeViewExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewExplorer.ImageIndex = 0;
            this.treeViewExplorer.ImageList = this.treeViewImageList;
            this.treeViewExplorer.Location = new System.Drawing.Point(0, 0);
            this.treeViewExplorer.Name = "treeViewExplorer";
            treeNode19.ImageIndex = 6;
            treeNode19.Name = "Node0";
            treeNode19.SelectedImageIndex = 6;
            treeNode19.Tag = "Exception";
            treeNode19.Text = "Exception";
            treeNode20.ImageIndex = 4;
            treeNode20.Name = "Node0";
            treeNode20.SelectedImageIndex = 4;
            treeNode20.Tag = "Handles";
            treeNode20.Text = "Handles";
            treeNode21.ImageIndex = 3;
            treeNode21.Name = "Node0";
            treeNode21.SelectedImageIndex = 3;
            treeNode21.Tag = "Memory";
            treeNode21.Text = "Memory";
            treeNode22.ImageIndex = 3;
            treeNode22.Name = "Node0";
            treeNode22.SelectedImageIndex = 3;
            treeNode22.Tag = "Memory64";
            treeNode22.Text = "Memory64";
            treeNode23.ImageIndex = 3;
            treeNode23.Name = "Node0";
            treeNode23.SelectedImageIndex = 3;
            treeNode23.Tag = "MemoryInfo";
            treeNode23.Text = "MemoryInfo";
            treeNode24.ImageIndex = 5;
            treeNode24.Name = "Node0";
            treeNode24.SelectedImageIndex = 5;
            treeNode24.Tag = "MiscInfo";
            treeNode24.Text = "MiscInfo";
            treeNode25.ImageIndex = 2;
            treeNode25.Name = "Node2";
            treeNode25.SelectedImageIndex = 2;
            treeNode25.Tag = "Modules";
            treeNode25.Text = "Modules";
            treeNode26.ImageIndex = 5;
            treeNode26.Name = "Node0";
            treeNode26.SelectedImageKey = "DialogID_6220_16x.png";
            treeNode26.Tag = "SystemInfo";
            treeNode26.Text = "SystemInfo";
            treeNode27.ImageIndex = 3;
            treeNode27.Name = "Node0";
            treeNode27.SelectedImageIndex = 3;
            treeNode27.Tag = "SystemMemoryInfo";
            treeNode27.Text = "SystemMemoryInfo";
            treeNode28.ImageIndex = 1;
            treeNode28.Name = "Node1";
            treeNode28.SelectedImageIndex = 1;
            treeNode28.Tag = "Threads";
            treeNode28.Text = "Threads";
            treeNode29.ImageIndex = 1;
            treeNode29.Name = "Node0";
            treeNode29.SelectedImageIndex = 1;
            treeNode29.Tag = "ThreadInfo";
            treeNode29.Text = "ThreadInfo";
            treeNode30.ImageIndex = 1;
            treeNode30.Name = "Node0";
            treeNode30.SelectedImageIndex = 1;
            treeNode30.Tag = "ThreadNames";
            treeNode30.Text = "ThreadNames";
            treeNode31.ImageIndex = 2;
            treeNode31.Name = "Node0";
            treeNode31.SelectedImageIndex = 2;
            treeNode31.Tag = "UnloadedModules";
            treeNode31.Text = "UnloadedModules";
            treeNode32.ImageIndex = 0;
            treeNode32.Name = "Node0";
            treeNode32.Tag = "Summary";
            treeNode32.Text = "<No minidump loaded>";
            this.treeViewExplorer.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode32});
            this.treeViewExplorer.SelectedImageIndex = 0;
            this.treeViewExplorer.ShowNodeToolTips = true;
            this.treeViewExplorer.Size = new System.Drawing.Size(223, 480);
            this.treeViewExplorer.TabIndex = 0;
			this.treeViewExplorer.AfterSelect += TreeViewExplorer_AfterSelect;
			// 
			// treeViewImageList
			// 
			this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "ActiveDocumentHost_6234.png");
            this.treeViewImageList.Images.SetKeyName(1, "thread_16xLG.png");
            this.treeViewImageList.Images.SetKeyName(2, "Assembly_6212.png");
            this.treeViewImageList.Images.SetKeyName(3, "MemoryWindow_6537.png");
            this.treeViewImageList.Images.SetKeyName(4, "Map_624.png");
            this.treeViewImageList.Images.SetKeyName(5, "DialogID_6220_16x.png");
            this.treeViewImageList.Images.SetKeyName(6, "Exception.png");
            // 
            // SnapshotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.splitContainer1);
            this.Name = "SnapshotControl";
            this.Size = new System.Drawing.Size(1117, 480);
            this.Load += new System.EventHandler(this.SnapshotControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		private void TreeViewExplorer_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (_dump.MiniDumpFile == null)
				return;

			CmdDisplayStream((string)e.Node.Tag);
		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewExplorer;
		private System.Windows.Forms.ImageList treeViewImageList;
	}
}
