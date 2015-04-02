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
        private OpenFileDialog _openContentDialog, _openSceneDialog;
        private SaveFileDialog _saveContentDialog, _saveSceneDialog;

        public readonly GUIExceptionHandler ExceptionHandler;

        #endregion

        #region Properties

        public EditorLogic Logic
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public MainForm(EditorLogic logic, string contentToOpen = null, string sceneToOpen = null)
        {
            this.Logic = logic;
            this.ExceptionHandler = new GUIExceptionHandler(this);
            InitializeComponent();
            UpdateTitle();
            SetSceneControlsEnabled(false);
            SetEditingControlsEnabled(false);

            _openContentDialog = new OpenFileDialog();
            _openSceneDialog   = new OpenFileDialog();
            _openContentDialog.Title = "Open content file";
            _openSceneDialog  .Title = "Open scene file";

            _saveContentDialog = new SaveFileDialog();
            _saveSceneDialog   = new SaveFileDialog();
            _openContentDialog.Title = "Save content file";
            _openSceneDialog  .Title = "Save scene file";

            _openSceneDialog  .Filter = _saveSceneDialog  .Filter =   "XML scene file (*.xml)|*.xml|All files (*.*)|*.*";
            _openContentDialog.Filter = _saveContentDialog.Filter = "XML content file (*.xml)|*.xml|All files (*.*)|*.*";

            viewportControl.AfterInitializeEvent += () =>
            {
                this.Logic.GameContentManager = viewportControl.GameContentManager;
                if (contentToOpen == null) return;

                if (!OpenContent(contentToOpen)) return;
                if (sceneToOpen != null)
                {
                    OpenScene(sceneToOpen);
                }
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
                case DialogResult.Yes:      return SaveScene();
                case DialogResult.No:       return true;
                case DialogResult.Cancel:   return false;
            }
            return true;
        }

        #region Content loaders

        private bool OpenContent(string path = null)
        {
            if (!IsSafeToUnload()) return false;
            if (path == null)
            {
                DialogResult result = this._openContentDialog.ShowDialog();
                if (result != DialogResult.OK) return false;
            }
            try
            {
                this.Logic.LoadContent(path ?? _openContentDialog.FileName);
                SetSceneControlsEnabled(true);
                UpdateTitle();
                contentTreeView.Nodes.Clear();

                TreeNode texturesNode = new TreeNode("Textures");
                foreach (var tex in this.Logic.XmlContent.Textures)
                {
                    texturesNode.Nodes.Add(new EditorTreeNode(tex.Name, tex));
                }
                contentTreeView.Nodes.Add(texturesNode);

                TreeNode materialsNode = new TreeNode("Materials");
                foreach (var mat in this.Logic.XmlContent.Materials)
                {
                    materialsNode.Nodes.Add(new EditorTreeNode(string.Format("ID: {0}", mat.Id), mat));
                }
                contentTreeView.Nodes.Add(materialsNode);

                TreeNode meshesNode = new TreeNode("Meshes");
                foreach (var mesh in this.Logic.XmlContent.Meshes)
                {
                    meshesNode.Nodes.Add(new EditorTreeNode(mesh.Path, mesh));
                }
                contentTreeView.Nodes.Add(meshesNode);

                GameObject sampleGameObject = new GameObject();
                sampleGameObject.AddComponent(new Renderer(sampleGameObject));
                sampleGameObject.renderer.MyMesh = Logic.XmlContent.Meshes[0];
                sampleGameObject.renderer.MyMesh.AssignRenderer(sampleGameObject.renderer);
                sampleGameObject.renderer.AssignMaterial(Logic.XmlContent.Materials[0]);
                sampleGameObject.renderer.MyEffect = Logic.GameContentManager.Load<Effect>("Effects/Shader");

                viewportControl.Reset();
                viewportControl.SampleObject = sampleGameObject;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves currently opened content file.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveContent()
        {
            try
            {
                this.Logic.SaveContent();
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }


        /// <summary>
        /// Saves opened content data in new file and switches to that file as current.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveContentAs()
        {
            DialogResult result = _saveContentDialog.ShowDialog();
            if (result != DialogResult.OK) return false;
            try
            {
                this.Logic.SaveContent(_saveContentDialog.FileName);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }

        #endregion


        #region Scene loaders

        private void OpenScene(string path = null)
        {
            if (!IsSafeToUnload()) return;
            if (path == null)
            {
                DialogResult result = this._openSceneDialog.ShowDialog();
                if (result != DialogResult.OK) return;
            }
            try
            {
                this.Logic.LoadScene(path ?? _openSceneDialog.FileName);
                SetEditingControlsEnabled(true);
                viewportControl.Reset();
                //viewportControl.SampleObject = sampleGameObject;

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// Saves currently opened scene.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveScene()
        {
            try
            {
                this.Logic.SaveScene();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }


        /// <summary>
        /// Saves opened scene data in new file and switches to that file as current.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveSceneAs()
        {
            DialogResult result = _saveSceneDialog.ShowDialog();
            if (result != DialogResult.OK) return false;
            try
            {
                this.Logic.SaveScene(_saveSceneDialog.FileName);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return false;
        }

        #endregion


        /// <summary>
        /// Updates title caption in editor application.
        /// </summary>
        private void UpdateTitle()
        {
            StringBuilder sb = new StringBuilder();
            string content = Logic.ContentSimpleName;
            string scene   = Logic.  SceneSimpleName;
            
            if (scene != null)
            {
                sb.Append(scene);
                if (this.Logic.History.AheadSaved != 0) sb.Append("* ");
                sb.Append(" - ");
            }
            if (content != null)
            {
                sb.Append(string.Format("({0}) - ", content));
            }
            sb.Append(this._appName);
            this.Text = sb.ToString();
        }

        /// <summary>
        /// Used after content loading to allow opening scene.
        /// Enables or disables controls for scene opening.
        /// </summary>
        /// <param name="mode">make controls enabled?</param>
        private void SetSceneControlsEnabled(bool mode)
        {
            openMenuItem        .Enabled = mode;
            openToolStripButton .Enabled = mode;
        }

        /// <summary>
        /// Used after opening scene to allow editing.
        /// </summary>
        /// <param name="mode">make controls enabled?</param>
        private void SetEditingControlsEnabled(bool mode)
        {
            saveMenuItem        .Enabled = mode;
            saveAsMenuItem      .Enabled = mode;
            saveToolStripButton .Enabled = mode;
        }

        public void ShowLogMessage(LoggerLevel level, string message)
        {
            if (level != LoggerLevel.Debug)
            {
                StringBuilder sb = new StringBuilder();
                if (level != LoggerLevel.Info)
                {
                    sb.Append(level).Append(": ");
                }
                sb.Append(message);
                statusBarLabel.Text = sb.ToString();
            }
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
            OpenScene();
        }

        private void SaveEvent(object sender, EventArgs e)
        {
            SaveScene();
        }

        private void SaveAsEvent(object sender, EventArgs e)
        {
            SaveSceneAs();
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
            OpenScene(files[0]);

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

        private void ContentTreeViewObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (contentTreeView.SelectedNode != null)
                    this.Logic.History.NewAction(contentTreeView.GetRemoveSelectedNodeCommand());
            }
        }

        private void ContentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode treeNode = e.Node;
            EditorTreeNode editorNode = treeNode as EditorTreeNode;
            propertyGrid.SelectedObject = (editorNode == null) ? null : editorNode.Data;
        }

        #endregion
        #endregion
    }
}
