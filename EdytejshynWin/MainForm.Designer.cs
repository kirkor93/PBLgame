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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sceneMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.undoCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.renderingModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.viewportControl = new Edytejshyn.GUI.ViewportControl();
            this.splitContainerRightHoriz = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sceneTreeView = new Edytejshyn.GUI.SceneTreeView();
            this.contentTreeView = new Edytejshyn.GUI.EditorTreeView();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.redoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.GizmoWorldToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.GizmoToolStripLabel = new System.Windows.Forms.ToolStripLabel();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneMenuBar,
            this.editMenuBar,
            this.viewMenuBar,
            this.helpMenuBar});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1176, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sceneMenuBar
            // 
            this.sceneMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.toolStripMenuItem1,
            this.exitMenuItem});
            this.sceneMenuBar.Name = "sceneMenuBar";
            this.sceneMenuBar.Size = new System.Drawing.Size(50, 20);
            this.sceneMenuBar.Text = "&Scene";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openMenuItem.Image")));
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
            this.saveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveMenuItem.Image")));
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
            this.undoMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoMenuItem.Image")));
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
            this.redoMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoMenuItem.Image")));
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeyDisplayString = "";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(166, 22);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // duplicateMenuItem
            // 
            this.duplicateMenuItem.Enabled = false;
            this.duplicateMenuItem.Name = "duplicateMenuItem";
            this.duplicateMenuItem.ShortcutKeyDisplayString = "";
            this.duplicateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateMenuItem.Size = new System.Drawing.Size(166, 22);
            this.duplicateMenuItem.Text = "&Duplicate";
            // 
            // viewMenuBar
            // 
            this.viewMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoCameraMenuItem,
            this.redoCameraMenuItem,
            this.toolStripMenuItem2,
            this.renderingModeMenuItem});
            this.viewMenuBar.Name = "viewMenuBar";
            this.viewMenuBar.Size = new System.Drawing.Size(44, 20);
            this.viewMenuBar.Text = "&View";
            // 
            // undoCameraMenuItem
            // 
            this.undoCameraMenuItem.Enabled = false;
            this.undoCameraMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoCameraMenuItem.Image")));
            this.undoCameraMenuItem.Name = "undoCameraMenuItem";
            this.undoCameraMenuItem.ShortcutKeyDisplayString = "Shift+Z";
            this.undoCameraMenuItem.Size = new System.Drawing.Size(224, 22);
            this.undoCameraMenuItem.Text = "&Undo camera move";
            this.undoCameraMenuItem.Click += new System.EventHandler(this.UndoCameraMenuItem_Click);
            // 
            // redoCameraMenuItem
            // 
            this.redoCameraMenuItem.Enabled = false;
            this.redoCameraMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoCameraMenuItem.Image")));
            this.redoCameraMenuItem.Name = "redoCameraMenuItem";
            this.redoCameraMenuItem.ShortcutKeyDisplayString = "Shift+Y";
            this.redoCameraMenuItem.Size = new System.Drawing.Size(224, 22);
            this.redoCameraMenuItem.Text = "&Redo camera move";
            this.redoCameraMenuItem.Click += new System.EventHandler(this.RedoCameraMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(221, 6);
            // 
            // renderingModeMenuItem
            // 
            this.renderingModeMenuItem.Name = "renderingModeMenuItem";
            this.renderingModeMenuItem.Size = new System.Drawing.Size(224, 22);
            this.renderingModeMenuItem.Text = "&Rendering mode";
            this.renderingModeMenuItem.DropDownOpening += new System.EventHandler(this.RenderingModeMenuItem_DropDownOpening);
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
            this.splitContainerVert.Panel1.Controls.Add(this.viewportControl);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.splitContainerRightHoriz);
            this.splitContainerVert.Size = new System.Drawing.Size(1176, 611);
            this.splitContainerVert.SplitterDistance = 704;
            this.splitContainerVert.TabIndex = 1;
            this.splitContainerVert.TabStop = false;
            // 
            // viewportControl
            // 
            this.viewportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewportControl.Location = new System.Drawing.Point(0, 0);
            this.viewportControl.Name = "viewportControl";
            this.viewportControl.Size = new System.Drawing.Size(704, 611);
            this.viewportControl.TabIndex = 2;
            this.viewportControl.Text = "XNA Viewport";
            // 
            // splitContainerRightHoriz
            // 
            this.splitContainerRightHoriz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRightHoriz.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRightHoriz.Name = "splitContainerRightHoriz";
            // 
            // splitContainerRightHoriz.Panel1
            // 
            this.splitContainerRightHoriz.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainerRightHoriz.Panel2
            // 
            this.splitContainerRightHoriz.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainerRightHoriz.Size = new System.Drawing.Size(468, 611);
            this.splitContainerRightHoriz.SplitterDistance = 264;
            this.splitContainerRightHoriz.TabIndex = 0;
            this.splitContainerRightHoriz.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sceneTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.contentTreeView);
            this.splitContainer1.Size = new System.Drawing.Size(264, 611);
            this.splitContainer1.SplitterDistance = 129;
            this.splitContainer1.TabIndex = 0;
            // 
            // sceneTreeView
            // 
            this.sceneTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneTreeView.HideSelection = false;
            this.sceneTreeView.Location = new System.Drawing.Point(0, 0);
            this.sceneTreeView.Name = "sceneTreeView";
            this.sceneTreeView.PathSeparator = "->";
            this.sceneTreeView.Size = new System.Drawing.Size(129, 611);
            this.sceneTreeView.TabIndex = 1;
            this.sceneTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SceneTreeView_AfterSelect);
            this.sceneTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SceneTreeView_NodeMouseClick);
            // 
            // contentTreeView
            // 
            this.contentTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentTreeView.Location = new System.Drawing.Point(0, 0);
            this.contentTreeView.Name = "contentTreeView";
            this.contentTreeView.PathSeparator = "->";
            this.contentTreeView.Size = new System.Drawing.Size(131, 611);
            this.contentTreeView.TabIndex = 0;
            this.contentTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ContentTreeView_AfterSelect);
            this.contentTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ContentTreeViewObjects_KeyDown);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(200, 611);
            this.propertyGrid.TabIndex = 1;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 660);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1176, 22);
            this.statusBar.TabIndex = 4;
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
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator,
            this.undoToolStripButton,
            this.redoToolStripButton,
            this.toolStripSeparator2,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.GizmoToolStripLabel,
            this.toolStripButton1,
            this.GizmoWorldToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1176, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open scene";
            this.openToolStripButton.Click += new System.EventHandler(this.OpenEvent);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Enabled = false;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save scene";
            this.saveToolStripButton.Click += new System.EventHandler(this.SaveEvent);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // undoToolStripButton
            // 
            this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoToolStripButton.Enabled = false;
            this.undoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripButton.Image")));
            this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoToolStripButton.Name = "undoToolStripButton";
            this.undoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.undoToolStripButton.Text = "&Undo";
            this.undoToolStripButton.Click += new System.EventHandler(this.UndoMenuItem_Click);
            // 
            // redoToolStripButton
            // 
            this.redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoToolStripButton.Enabled = false;
            this.redoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripButton.Image")));
            this.redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoToolStripButton.Name = "redoToolStripButton";
            this.redoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.redoToolStripButton.Text = "&Redo";
            this.redoToolStripButton.ToolTipText = "Redo";
            this.redoToolStripButton.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Enabled = false;
            this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Enabled = false;
            this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "&Copy";
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Enabled = false;
            this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "&Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // GizmoWorldToolStripButton
            // 
            this.GizmoWorldToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.GizmoWorldToolStripButton.Enabled = false;
            this.GizmoWorldToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("GizmoWorldToolStripButton.Image")));
            this.GizmoWorldToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GizmoWorldToolStripButton.Name = "GizmoWorldToolStripButton";
            this.GizmoWorldToolStripButton.Size = new System.Drawing.Size(43, 22);
            this.GizmoWorldToolStripButton.Text = "World";
            this.GizmoWorldToolStripButton.ToolTipText = "Gizmo world space";
            // 
            // GizmoToolStripLabel
            // 
            this.GizmoToolStripLabel.Name = "GizmoToolStripLabel";
            this.GizmoToolStripLabel.Size = new System.Drawing.Size(44, 22);
            this.GizmoToolStripLabel.Text = "Gizmo:";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Checked = true;
            this.toolStripButton1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(39, 22);
            this.toolStripButton1.Text = "Local";
            this.toolStripButton1.ToolTipText = "Gizmo local space";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 682);
            this.Controls.Add(this.splitContainerVert);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusBar);
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
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sceneMenuBar;
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
        private GUI.EditorTreeView contentTreeView;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private GUI.ViewportControl viewportControl;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton undoToolStripButton;
        private System.Windows.Forms.ToolStripButton redoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private GUI.SceneTreeView sceneTreeView;
        private System.Windows.Forms.ToolStripMenuItem viewMenuBar;
        public System.Windows.Forms.ToolStripMenuItem undoCameraMenuItem;
        public System.Windows.Forms.ToolStripMenuItem redoCameraMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem renderingModeMenuItem;
        private System.Windows.Forms.ToolStripButton GizmoWorldToolStripButton;
        private System.Windows.Forms.ToolStripLabel GizmoToolStripLabel;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}

