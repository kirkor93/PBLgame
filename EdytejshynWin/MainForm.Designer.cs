﻿namespace Edytejshyn
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
            this.reloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.gridSnappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.snapToGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.undoCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.renderingModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.updateSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zeroTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forward01sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMaterialsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuBar = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.fixAllCollidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.viewportControl = new Edytejshyn.GUI.ViewportControl();
            this.splitContainerRightHoriz = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sceneTreeView = new Edytejshyn.GUI.SceneTreeView();
            this.contentTreeView = new Edytejshyn.GUI.EditorTreeView();
            this.splitContainerProperty = new System.Windows.Forms.SplitContainer();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.collisionTreeView = new Edytejshyn.GUI.CollisionTreeView();
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
            this.gizmoToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.gizmoSpaceToolStripButton = new System.Windows.Forms.ToolStripButton();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerProperty)).BeginInit();
            this.splitContainerProperty.Panel1.SuspendLayout();
            this.splitContainerProperty.Panel2.SuspendLayout();
            this.splitContainerProperty.SuspendLayout();
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
            this.contentToolStripMenuItem,
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
            this.reloadMenuItem,
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
            // reloadMenuItem
            // 
            this.reloadMenuItem.Enabled = false;
            this.reloadMenuItem.Name = "reloadMenuItem";
            this.reloadMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reloadMenuItem.Text = "&Reload";
            this.reloadMenuItem.Click += new System.EventHandler(this.reloadMenuItem_Click);
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
            this.duplicateMenuItem,
            this.toolStripMenuItem3,
            this.gridSnappingToolStripMenuItem,
            this.snapToGridToolStripMenuItem});
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
            this.undoMenuItem.Size = new System.Drawing.Size(179, 22);
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
            this.redoMenuItem.Size = new System.Drawing.Size(179, 22);
            this.redoMenuItem.Text = "&Redo";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // duplicateMenuItem
            // 
            this.duplicateMenuItem.Name = "duplicateMenuItem";
            this.duplicateMenuItem.ShortcutKeyDisplayString = "";
            this.duplicateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateMenuItem.Size = new System.Drawing.Size(179, 22);
            this.duplicateMenuItem.Text = "&Duplicate";
            this.duplicateMenuItem.Click += new System.EventHandler(this.duplicateMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(176, 6);
            // 
            // gridSnappingToolStripMenuItem
            // 
            this.gridSnappingToolStripMenuItem.Name = "gridSnappingToolStripMenuItem";
            this.gridSnappingToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.gridSnappingToolStripMenuItem.Text = "&Grid && Snapping..";
            this.gridSnappingToolStripMenuItem.Click += new System.EventHandler(this.gridSnappingToolStripMenuItem_Click);
            // 
            // snapToGridToolStripMenuItem
            // 
            this.snapToGridToolStripMenuItem.CheckOnClick = true;
            this.snapToGridToolStripMenuItem.Name = "snapToGridToolStripMenuItem";
            this.snapToGridToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.snapToGridToolStripMenuItem.Text = "Always &Snap to Grid";
            this.snapToGridToolStripMenuItem.Click += new System.EventHandler(this.snapToGridToolStripMenuItem_Click);
            // 
            // viewMenuBar
            // 
            this.viewMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoCameraMenuItem,
            this.redoCameraMenuItem,
            this.toolStripMenuItem2,
            this.renderingModeMenuItem,
            this.toolStripMenuItem4,
            this.updateSceneToolStripMenuItem});
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
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(221, 6);
            // 
            // updateSceneToolStripMenuItem
            // 
            this.updateSceneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zeroTimeToolStripMenuItem,
            this.forward01sToolStripMenuItem});
            this.updateSceneToolStripMenuItem.Name = "updateSceneToolStripMenuItem";
            this.updateSceneToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.updateSceneToolStripMenuItem.Text = "Update scene";
            // 
            // zeroTimeToolStripMenuItem
            // 
            this.zeroTimeToolStripMenuItem.Name = "zeroTimeToolStripMenuItem";
            this.zeroTimeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.zeroTimeToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.zeroTimeToolStripMenuItem.Text = "Zero time";
            this.zeroTimeToolStripMenuItem.Click += new System.EventHandler(this.zeroTimeToolStripMenuItem_Click);
            // 
            // forward01sToolStripMenuItem
            // 
            this.forward01sToolStripMenuItem.Name = "forward01sToolStripMenuItem";
            this.forward01sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.forward01sToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.forward01sToolStripMenuItem.Text = "Forward 0.1s";
            this.forward01sToolStripMenuItem.Click += new System.EventHandler(this.forward01sToolStripMenuItem_Click);
            // 
            // contentToolStripMenuItem
            // 
            this.contentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateMaterialsMenuItem});
            this.contentToolStripMenuItem.Name = "contentToolStripMenuItem";
            this.contentToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.contentToolStripMenuItem.Text = "Content";
            // 
            // generateMaterialsMenuItem
            // 
            this.generateMaterialsMenuItem.Name = "generateMaterialsMenuItem";
            this.generateMaterialsMenuItem.Size = new System.Drawing.Size(172, 22);
            this.generateMaterialsMenuItem.Text = "Generate materials";
            this.generateMaterialsMenuItem.Click += new System.EventHandler(this.generateMaterialsMenuItem_Click);
            // 
            // helpMenuBar
            // 
            this.helpMenuBar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem,
            this.toolStripMenuItem5,
            this.fixAllCollidersToolStripMenuItem});
            this.helpMenuBar.Name = "helpMenuBar";
            this.helpMenuBar.Size = new System.Drawing.Size(44, 20);
            this.helpMenuBar.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
            // 
            // fixAllCollidersToolStripMenuItem
            // 
            this.fixAllCollidersToolStripMenuItem.Enabled = false;
            this.fixAllCollidersToolStripMenuItem.Name = "fixAllCollidersToolStripMenuItem";
            this.fixAllCollidersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fixAllCollidersToolStripMenuItem.Text = "Fix all colliders";
            this.fixAllCollidersToolStripMenuItem.Click += new System.EventHandler(this.fixAllCollidersToolStripMenuItem_Click);
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
            this.viewportControl.Gizmo = null;
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
            this.splitContainerRightHoriz.Panel2.Controls.Add(this.splitContainerProperty);
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
            this.sceneTreeView.AllowDrop = true;
            this.sceneTreeView.DestinationNode = null;
            this.sceneTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneTreeView.HideSelection = false;
            this.sceneTreeView.Location = new System.Drawing.Point(0, 0);
            this.sceneTreeView.MovedNode = null;
            this.sceneTreeView.Name = "sceneTreeView";
            this.sceneTreeView.PathSeparator = "->";
            this.sceneTreeView.Size = new System.Drawing.Size(129, 611);
            this.sceneTreeView.TabIndex = 1;
            this.sceneTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.SceneTreeView_ItemDrag);
            this.sceneTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SceneTreeView_AfterSelect);
            this.sceneTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SceneTreeView_NodeMouseClick);
            this.sceneTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.SceneTreeView_DragDrop);
            this.sceneTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.SceneTreeView_DragEnter);
            this.sceneTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.SceneTreeView_DragOver);
            this.sceneTreeView.DragLeave += new System.EventHandler(this.SceneTreeView_DragLeave);
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
            // splitContainerProperty
            // 
            this.splitContainerProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerProperty.Location = new System.Drawing.Point(0, 0);
            this.splitContainerProperty.Name = "splitContainerProperty";
            this.splitContainerProperty.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerProperty.Panel1
            // 
            this.splitContainerProperty.Panel1.Controls.Add(this.propertyGrid);
            // 
            // splitContainerProperty.Panel2
            // 
            this.splitContainerProperty.Panel2.Controls.Add(this.collisionTreeView);
            this.splitContainerProperty.Size = new System.Drawing.Size(200, 611);
            this.splitContainerProperty.SplitterDistance = 412;
            this.splitContainerProperty.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(200, 412);
            this.propertyGrid.TabIndex = 1;
            // 
            // collisionTreeView
            // 
            this.collisionTreeView.Collision = null;
            this.collisionTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collisionTreeView.Location = new System.Drawing.Point(0, 0);
            this.collisionTreeView.Name = "collisionTreeView";
            this.collisionTreeView.Size = new System.Drawing.Size(200, 195);
            this.collisionTreeView.TabIndex = 0;
            this.collisionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.CollisionTreeView_AfterSelect);
            this.collisionTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.CollisionTreeView_NodeMouseClick);
            this.collisionTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.collisionTreeView_MouseDown);
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
            this.gizmoToolStripLabel,
            this.gizmoSpaceToolStripButton});
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
            // gizmoToolStripLabel
            // 
            this.gizmoToolStripLabel.Name = "gizmoToolStripLabel";
            this.gizmoToolStripLabel.Size = new System.Drawing.Size(44, 22);
            this.gizmoToolStripLabel.Text = "Gizmo:";
            // 
            // gizmoSpaceToolStripButton
            // 
            this.gizmoSpaceToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.gizmoSpaceToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("gizmoSpaceToolStripButton.Image")));
            this.gizmoSpaceToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gizmoSpaceToolStripButton.Name = "gizmoSpaceToolStripButton";
            this.gizmoSpaceToolStripButton.Size = new System.Drawing.Size(39, 22);
            this.gizmoSpaceToolStripButton.Text = "Local";
            this.gizmoSpaceToolStripButton.ToolTipText = "Gizmo space";
            this.gizmoSpaceToolStripButton.Click += new System.EventHandler(this.gizmoSpaceToolStripButton_Click);
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
            this.splitContainerProperty.Panel1.ResumeLayout(false);
            this.splitContainerProperty.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerProperty)).EndInit();
            this.splitContainerProperty.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripLabel gizmoToolStripLabel;
        private System.Windows.Forms.ToolStripButton gizmoSpaceToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem gridSnappingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem snapToGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateMaterialsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem updateSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zeroTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forward01sToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerProperty;
        private GUI.CollisionTreeView collisionTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem fixAllCollidersToolStripMenuItem;
    }
}

