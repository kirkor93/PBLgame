using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Edytejshyn.GUI;
using Edytejshyn.Logic;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn
{
    public partial class MainForm : Form
    {

        #region Variables
        private readonly string _appName = "Edytejszyn (Build " + Assembly.GetExecutingAssembly().GetName().Version + ")";
        private OpenFileDialog _openDialog;
        private SaveFileDialog _saveDialog;

        public readonly GUIExceptionHandler ExceptionHandler;
        private bool _hotkeysDisabled;

        #endregion

        #region Properties

        public EditorLogic Logic
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public MainForm(EditorLogic logic, string fileToOpen = null)
        {
            this.Logic = logic;
            this.ExceptionHandler = new GUIExceptionHandler(this);
            InitializeComponent();
            UpdateTitle();
            SetFileControlsEnabled(false);
            _openDialog = new OpenFileDialog();
            _saveDialog = new SaveFileDialog();
            _openDialog.Filter = _saveDialog.Filter = "XML file (*.xml)|*.xml|All files (*.*)|*.*";
            _openDialog.InitialDirectory = ".";

            viewportControl.AfterInitializeEvent += () =>
            {
                this.Logic.GameContent = viewportControl.GameContent;
                if (fileToOpen != null)
                    OpenFile(fileToOpen);
            };

            this.Logic.Logger.LogEvent += ShowLogMessage;
            this.Logic.History.UpdateEvent += UpdateHistory;
            this.Logic.History.Clear();
        }


        /// <summary>
        /// Call me before possible data lose like closing program or loading other file.
        /// If there are changes, user will be asked for saving.
        /// </summary>
        /// <returns>true if safe to close</returns>
        private bool IsSafeToUnload()
        {
            if (Logic.History.AheadSaved == 0) return true;
            DialogResult result = MessageBox.Show("Data has been changed. Shall I save?", "Possible data lose", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes:      return SaveFile();
                case DialogResult.No:       return true;
                case DialogResult.Cancel:   return false;
            }
            return true;
        }

        private void OpenFile(string path = null)
        {
            if (!IsSafeToUnload()) return;
            if (path == null)
            {
                DialogResult result = this._openDialog.ShowDialog();
                if (result != DialogResult.OK) return;
            }
            try
            {
                this.Logic.LoadFile(path ?? _openDialog.FileName);
                SetFileControlsEnabled(true);
                hierarchyTreeView.Nodes.Clear();
                hierarchyTreeView.Nodes.Add("!! PREVIEW ONLY !!");


                TreeNode texturesNode = new TreeNode("Textures");
                foreach (var tex in this.Logic.Content.Textures)
                {
                    texturesNode.Nodes.Add(new EditorTreeNode(tex.Name, tex));
                }
                hierarchyTreeView.Nodes.Add(texturesNode);

                TreeNode materialsNode = new TreeNode("Materials");
                foreach (var mat in this.Logic.Content.Materials)
                {
                    materialsNode.Nodes.Add(new EditorTreeNode(string.Format("ID: {0}", mat.Id), mat));
                }
                hierarchyTreeView.Nodes.Add(materialsNode);

                TreeNode meshesNode = new TreeNode("Meshes");
                foreach (var mesh in this.Logic.Content.Meshes)
                {
                    meshesNode.Nodes.Add(new EditorTreeNode(mesh.Path, mesh));
                }
                hierarchyTreeView.Nodes.Add(meshesNode);

                GameObject sampleGameObject = new GameObject();
                sampleGameObject.AddComponent(new Renderer(sampleGameObject));
                sampleGameObject.renderer.MyMesh = Logic.Content.Meshes[0];
                sampleGameObject.renderer.MyMesh.AssignRenderer(sampleGameObject.renderer);
                sampleGameObject.renderer.AssignMaterial(Logic.Content.Materials[0]);
                sampleGameObject.renderer.MyEffect = Logic.GameContent.Load<Effect>("Effects/Shader");

                viewportControl.Reset();
                viewportControl.SampleObject = sampleGameObject;

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// Saves currently opened file.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveFile()
        {
            try
            {
                this.Logic.SaveFile();
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }


        /// <summary>
        /// Saves opened data in new file and switches to that file as current.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveFileAs()
        {
            DialogResult result = _saveDialog.ShowDialog();
            if (result != DialogResult.OK) return false;
            try
            {
                this.Logic.SaveFile(_saveDialog.FileName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }

        /// <summary>
        /// Updates title caption in editor application.
        /// </summary>
        private void UpdateTitle()
        {
            StringBuilder sb = new StringBuilder();
            string file = Logic.SimpleFileName;
            if (file != null)
            {
                sb.Append(file);
                if (this.Logic.History.AheadSaved != 0) sb.Append("* ");
                sb.Append(" - ");
            }
            sb.Append(this._appName);
            this.Text = sb.ToString();
        }

        private void SetFileControlsEnabled(bool mode)
        {
            saveMenuItem       .Enabled = mode;
            saveAsMenuItem     .Enabled = mode;
            saveToolStripButton.Enabled = mode;
        }

        public void ShowLogMessage(LoggerLevel level, string message)
        {
            if(level != LoggerLevel.Debug)
                statusBarLabel.Text = string.Format("{0}: {1}", level, message);
            statusBarLabel.ForeColor = level.GetColor(DefaultForeColor);
        }

        /// <summary>
        /// Update menu entries of Undo and Redo.
        /// </summary>
        /// <param name="manager">Reference to calling history manager</param>
        public void UpdateHistory(HistoryManager manager)
        {
            // I hate duplicated code, so i made that:

            var entries = new[]
            {
                new {cmd = manager.NextUndo, item = undoMenuItem, toolstrip = undoToolStripButton, caption = "&Undo"},
                new {cmd = manager.NextRedo, item = redoMenuItem, toolstrip = redoToolStripButton, caption = "&Redo"},
            };

            foreach (var entry in entries)
            {
                if (entry.cmd != null)
                {
                    entry.item.Enabled = entry.toolstrip.Enabled = true;
                    entry.item.Text = entry.toolstrip.Text = string.Format("{0}: {1}", entry.caption, entry.cmd.Description);
                }
                else
                {
                    entry.item.Enabled = entry.toolstrip.Enabled = false;
                    entry.item.Text = entry.toolstrip.Text = entry.caption;
                }
            }
            UpdateTitle();
        }

        #region Events
        private void ExitEvent(object sender, EventArgs e)
        {
            Close();
        }

        private void OpenEvent(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveEvent(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveAsEvent(object sender, EventArgs e)
        {
            SaveFileAs();
        }


        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.Instance.ShowDialog();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            OpenFile(files[0]);

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsSafeToUnload())
            {
                e.Cancel = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void UndoMenuItem_Click(object sender, EventArgs e)
        {
            Logic.History.Undo();
        }

        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            Logic.History.Redo();
        }

        private void HierarchyTreeViewObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (hierarchyTreeView.SelectedNode != null)
                    this.Logic.History.NewAction(hierarchyTreeView.GetRemoveSelectedNodeCommand());
            }
        }

        private void HierarchyTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode treeNode = e.Node;
            EditorTreeNode editorNode = treeNode as EditorTreeNode;
            propertyGrid.SelectedObject = (editorNode == null) ? null : editorNode.Data;
        }

        #endregion
        #endregion
    }
}
