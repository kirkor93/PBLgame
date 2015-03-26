namespace Edytejshyn
{
    partial class MainForm
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
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node11");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node12");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node1", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node6");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node4", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node9");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Node10");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node7", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11,
            treeNode12});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.viewport = new Edytejshyn.GUI.ViewportControl();
            this.splitContainerRightHoriz = new System.Windows.Forms.SplitContainer();
            this.hierarchyTreeView = new System.Windows.Forms.TreeView();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRightHoriz)).BeginInit();
            this.splitContainerRightHoriz.Panel1.SuspendLayout();
            this.splitContainerRightHoriz.Panel2.SuspendLayout();
            this.splitContainerRightHoriz.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuBar,
            this.editMenuBar,
            this.helpMenuBar});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(808, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuBar
            // 
            this.fileMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.toolStripMenuItem1,
            this.exitMenuItem});
            this.fileMenuBar.Name = "fileMenuBar";
            this.fileMenuBar.Size = new System.Drawing.Size(37, 20);
            this.fileMenuBar.Text = "&File";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeyDisplayString = "";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openMenuItem.Text = "&Open..";
            this.openMenuItem.Click += new System.EventHandler(this.OpenEvent);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Enabled = false;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeyDisplayString = "";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveMenuItem.Text = "&Save";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveEvent);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Enabled = false;
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.saveAsMenuItem.ShowShortcutKeys = false;
            this.saveAsMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsMenuItem.Text = "&Save As..";
            this.saveAsMenuItem.Click += new System.EventHandler(this.SaveAsEvent);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMenuItem.Text = "&Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitEvent);
            // 
            // editMenuBar
            // 
            this.editMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.duplicateMenuItem});
            this.editMenuBar.Name = "editMenuBar";
            this.editMenuBar.Size = new System.Drawing.Size(39, 20);
            this.editMenuBar.Text = "&Edit";
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeyDisplayString = "";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(166, 22);
            this.undoMenuItem.Text = "&Undo";
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItem_Click);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeyDisplayString = "";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(166, 22);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // duplicateMenuItem
            // 
            this.duplicateMenuItem.Name = "duplicateMenuItem";
            this.duplicateMenuItem.ShortcutKeyDisplayString = "";
            this.duplicateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateMenuItem.Size = new System.Drawing.Size(166, 22);
            this.duplicateMenuItem.Text = "&Duplicate";
            // 
            // helpMenuBar
            // 
            this.helpMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuBar.Name = "helpMenuBar";
            this.helpMenuBar.Size = new System.Drawing.Size(44, 20);
            this.helpMenuBar.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // splitContainerVert
            // 
            this.splitContainerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVert.Location = new System.Drawing.Point(0, 49);
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.viewport);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.splitContainerRightHoriz);
            this.splitContainerVert.Size = new System.Drawing.Size(808, 495);
            this.splitContainerVert.SplitterDistance = 519;
            this.splitContainerVert.TabIndex = 1;
            // 
            // viewport
            // 
            this.viewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewport.Location = new System.Drawing.Point(0, 0);
            this.viewport.Name = "viewport";
            this.viewport.Size = new System.Drawing.Size(519, 495);
            this.viewport.TabIndex = 0;
            this.viewport.TabStop = false;
            this.viewport.Text = "XNA Viewport";
            this.viewport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Viewport_MouseMove);
            // 
            // splitContainerRightHoriz
            // 
            this.splitContainerRightHoriz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRightHoriz.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRightHoriz.Name = "splitContainerRightHoriz";
            this.splitContainerRightHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerRightHoriz.Panel1
            // 
            this.splitContainerRightHoriz.Panel1.Controls.Add(this.hierarchyTreeView);
            // 
            // splitContainerRightHoriz.Panel2
            // 
            this.splitContainerRightHoriz.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainerRightHoriz.Size = new System.Drawing.Size(285, 495);
            this.splitContainerRightHoriz.SplitterDistance = 179;
            this.splitContainerRightHoriz.TabIndex = 0;
            // 
            // hierarchyTreeView
            // 
            this.hierarchyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hierarchyTreeView.Location = new System.Drawing.Point(0, 0);
            this.hierarchyTreeView.Name = "hierarchyTreeView";
            treeNode1.Name = "Node11";
            treeNode1.Text = "Node11";
            treeNode2.Name = "Node12";
            treeNode2.Text = "Node12";
            treeNode3.Name = "Node1";
            treeNode3.Text = "Node1";
            treeNode4.Name = "Node2";
            treeNode4.Text = "Node2";
            treeNode5.Name = "Node3";
            treeNode5.Text = "Node3";
            treeNode6.Name = "Node0";
            treeNode6.Text = "Node0";
            treeNode7.Name = "Node5";
            treeNode7.Text = "Node5";
            treeNode8.Name = "Node6";
            treeNode8.Text = "Node6";
            treeNode9.Name = "Node4";
            treeNode9.Text = "Node4";
            treeNode10.Name = "Node8";
            treeNode10.Text = "Node8";
            treeNode11.Name = "Node9";
            treeNode11.Text = "Node9";
            treeNode12.Name = "Node10";
            treeNode12.Text = "Node10";
            treeNode13.Name = "Node7";
            treeNode13.Text = "Node7";
            this.hierarchyTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode9,
            treeNode13});
            this.hierarchyTreeView.Size = new System.Drawing.Size(285, 179);
            this.hierarchyTreeView.TabIndex = 0;
            this.hierarchyTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeViewObjects_KeyDown);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(285, 312);
            this.propertyGrid.TabIndex = 0;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 522);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(808, 22);
            this.statusBar.TabIndex = 2;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusBarLabel
            // 
            this.statusBarLabel.Name = "statusBarLabel";
            this.statusBarLabel.Size = new System.Drawing.Size(221, 17);
            this.statusBarLabel.Text = "Welcome to Edytejszyn. Have a nice day.";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(808, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 544);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.splitContainerVert);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Edytejshyn v.xxxx";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.splitContainerRightHoriz.Panel1.ResumeLayout(false);
            this.splitContainerRightHoriz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRightHoriz)).EndInit();
            this.splitContainerRightHoriz.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuBar;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuBar;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuBar;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.SplitContainer splitContainerRightHoriz;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusBarLabel;
        private System.Windows.Forms.TreeView hierarchyTreeView;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private GUI.ViewportControl viewport;
    }
}

