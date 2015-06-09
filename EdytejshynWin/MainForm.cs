using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Edytejshyn.GUI;
using Edytejshyn.GUI.XNA;
using Edytejshyn.Logic;
using Edytejshyn.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Edytejshyn
{
    public partial class MainForm : Form
    {

        #region Variables
        private readonly string _appName = "Edytejszyn (Build " + Assembly.GetExecutingAssembly().GetName().Version + ")";
        private OpenFileDialog _openContentDialog, _openSceneDialog;
        private SaveFileDialog _saveContentDialog, _saveSceneDialog;
        private GridSnapDialog _gridSnapDialog;

        public readonly GUIExceptionHandler ExceptionHandler;

        #endregion

        #region Properties

        public EditorLogic Logic { get; private set; }
        public SelectionManager SelectionManager { get; private set; }
        public ViewportControl ViewportControl { get { return viewportControl; } }

        public IDrawerStrategy RealisticDrawerStrategy { get; private set; }
        public IDrawerStrategy BasicDrawerStrategy { get; private set; }
        public IDrawerStrategy CurrentDrawerStrategy { get; set; }
        public bool SnapEnabled { get { return snapToGridToolStripMenuItem.Checked; } }

        #endregion

        #region Methods

        public MainForm(EditorLogic logic, string contentToOpen = null, string sceneToOpen = null, bool basicRender = false)
        {
            this.Logic = logic;
            this.ExceptionHandler = new GUIExceptionHandler(this);
            InitializeComponent();
            UpdateTitle();
            SetSceneControlsEnabled(false);
            SetEditingControlsEnabled(false);
            //KeyPreview = true;

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

            sceneTreeView.MainForm = this;
            viewportControl.MainForm = this;

            viewportControl.AfterInitializeEvent += () =>
            {
                SelectionManager = new SelectionManager(Logic, sceneTreeView, viewportControl);
                Logic.SelectionManager = SelectionManager;
                BasicDrawerStrategy = new BasicDrawerStrategy(viewportControl.GraphicsDevice);
                RealisticDrawerStrategy = new RealisticDrawerStrategy();
                CurrentDrawerStrategy = basicRender ? BasicDrawerStrategy : RealisticDrawerStrategy;

                var dropdowns = new[]
                {
                    new {text = "&Realistic", strategy = RealisticDrawerStrategy},
                    new {text = "&Basic",     strategy = BasicDrawerStrategy},
                };

                foreach (var entry in dropdowns)
                {
                    DrawerStrategyMenuItem menuItem = new DrawerStrategyMenuItem(entry.text, entry.strategy);

                    menuItem.Click += delegate
                    {
                        CurrentDrawerStrategy = menuItem.Strategy;
                    };

                    renderingModeMenuItem.DropDownItems.Add(menuItem);
                }

                viewportControl.Gizmo.ActiveSpaceChanged += delegate
                {
                    gizmoSpaceToolStripButton.Text = (viewportControl.Gizmo.ActiveSpace == TransformSpace.Global)
                        ? "Global"
                        : "Local";
                    viewportControl.Invalidate();
                };

                this.Logic.GameContentManager = viewportControl.GameContentManager;
                viewportControl.CameraHistory.UpdateEvent += UpdateCameraHistory;
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
                foreach (var tex in this.Logic.ResourceManager.Textures)
                {   
                    // TODO trim Textures\ prefix
                    texturesNode.Nodes.Add(new EditorTreeNode(tex.Name, tex));
                }
                contentTreeView.Nodes.Add(texturesNode);

                TreeNode materialsNode = new TreeNode("Materials");
                foreach (var mat in this.Logic.ResourceManager.Materials)
                {
                    materialsNode.Nodes.Add(new EditorTreeNode(mat.ToString(), mat));
                }
                contentTreeView.Nodes.Add(materialsNode);

                TreeNode meshesNode = new TreeNode("Meshes");
                foreach (var mesh in this.Logic.ResourceManager.Meshes)
                {
                    meshesNode.Nodes.Add(new EditorTreeNode(mesh.ToString(), mesh));
                }
                contentTreeView.Nodes.Add(meshesNode);

                TreeNode effectsNode = new TreeNode("Shader Effects");
                foreach (var fx in this.Logic.ResourceManager.ShaderEffects)
                {
                    effectsNode.Nodes.Add(new EditorTreeNode(fx.Name, fx));
                }
                contentTreeView.Nodes.Add(effectsNode);

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

        private void OpenScene(string path = null, bool reset = true)
        {
            if (!IsSafeToUnload()) return;
            if (path == null)
            {
                DialogResult result = this._openSceneDialog.ShowDialog();
                if (result != DialogResult.OK) return;
                path = _openSceneDialog.FileName;
            }
            try
            {
                this.Logic.LoadScene(path);
                Logic.WrappedScene.TreeView = sceneTreeView;
                SetEditingControlsEnabled(true);
                if(reset) viewportControl.Reset();

                sceneTreeView.ReloadTree();
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
            reloadMenuItem      .Enabled = mode;
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
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Update menu entries for Camera Undo and Redo.
        /// </summary>
        /// <param name="history">Reference to calling history manager</param>
        public void UpdateCameraHistory(CameraHistory history)
        {
            undoCameraMenuItem.Enabled = viewportControl.CameraHistory.CanUndo;
            redoCameraMenuItem.Enabled = viewportControl.CameraHistory.CanRedo;
        }

        #region Events
        
        #region MainForm & History
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


        public void UndoCameraMenuItem_Click(object sender, EventArgs e)
        {
            viewportControl.CameraHistory.Undo();
        }

        public void RedoCameraMenuItem_Click(object sender, EventArgs e)
        {
            viewportControl.CameraHistory.Redo();
        }
        #endregion

        #region ContentTreeView
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
            EditorTreeNode node = e.Node as EditorTreeNode;
            propertyGrid.SelectedObject = (node == null) ? null : node.Data;
            propertyGrid.ExpandAllGridItems();
        }
        #endregion

        #region SceneTreeView
        private void SceneTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SceneTreeNode node = e.Node as SceneTreeNode;
            SelectionManager.SelectOnly((node == null) ? null : node.WrappedGameObject, false);
            propertyGrid.SelectedObject = (node == null) ? null : node.WrappedGameObject;
            collisionTreeView.Collision = (node == null) ? null : node.WrappedGameObject.Collision;
            collisionTreeView.ExpandAll();
            if (node == null) return;

            ContextMenuStrip popup = new ContextMenuStrip();
            if (node.WrappedGameObject.Renderer == null)
            {
                popup.Items.Add("Add Rendererer").Click += delegate
                {
                    node.WrappedGameObject.NewRenderer();
                    SceneTreeView_AfterSelect(this, new TreeViewEventArgs(node));
                };
            }
            else
            {
                popup.Items.Add("Remove Rendererer").Click += delegate
                {
                    node.WrappedGameObject.RemoveRenderer();
                    SceneTreeView_AfterSelect(this, new TreeViewEventArgs(node));
                };
            }

            if (node.WrappedGameObject.Collision == null)
            {
                popup.Items.Add("Add Collision").Click += delegate
                {
                    node.WrappedGameObject.NewCollision();
                    SceneTreeView_AfterSelect(this, new TreeViewEventArgs(node));
                };
            }
            else
            {
                popup.Items.Add("Remove Collision").Click += delegate
                {
                    node.WrappedGameObject.RemoveCollision();
                    SceneTreeView_AfterSelect(this, new TreeViewEventArgs(node));
                };
            }
            sceneTreeView.ContextMenuStrip = popup;

            propertyGrid.ExpandAllGridItems();
        }

        private void SceneTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                sceneTreeView.SelectedNode = e.Node;
            }
            else if(e.Button == MouseButtons)
                SceneTreeView_AfterSelect(sender, new TreeViewEventArgs(e.Node, TreeViewAction.ByMouse));
        }

        private void SceneTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            sceneTreeView.MovedNode = (SceneTreeNode) e.Item;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void SceneTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (sceneTreeView.MovedNode != null) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void SceneTreeView_DragDrop(object sender, DragEventArgs e)
        {
            GameObjectWrapper newParent;
            SceneTreeNode movedNode = sceneTreeView.MovedNode;
            sceneTreeView.MovedNode = null;
            if (sceneTreeView.DestinationNode == null)
            {
                if (movedNode.Parent == null) return;
                newParent = null;
            }
            else
            {
                sceneTreeView.DestinationNode.BackColor = sceneTreeView.BackColor;
                newParent = sceneTreeView.DestinationNode.WrappedGameObject;
                if (newParent is LightWrapper)
                {
                    MessageBox.Show("Lights are impotents, so cannot be parents (even adopted). Because fuck you, that's why (kirek hasn't implemented that)", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (sceneTreeView.DestinationNode == movedNode 
                    || newParent == movedNode.WrappedGameObject.Parent
                    || sceneTreeView.DestinationNode.WrappedGameObject.HasAncestor(movedNode.WrappedGameObject)) return;
            }

            Logic.WrappedScene.ReparentNode(movedNode.WrappedGameObject, newParent);
            
        }



        private void SceneTreeView_DragLeave(object sender, EventArgs e)
        {
            
        }

        private void SceneTreeView_DragOver(object sender, DragEventArgs e)
        {
            SceneTreeNode oldNode = sceneTreeView.DestinationNode;
            sceneTreeView.DestinationNode = (SceneTreeNode) sceneTreeView.GetNodeAt(sceneTreeView.PointToClient(new Point(e.X, e.Y)));
            if (oldNode == sceneTreeView.DestinationNode) return;
            
            if (oldNode != null) 
                oldNode.BackColor = sceneTreeView.BackColor;
            if (sceneTreeView.DestinationNode != null) 
                sceneTreeView.DestinationNode.BackColor = Color.LawnGreen;
        }
        #endregion

        #region Viewport & Co.
        private void RenderingModeMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            foreach (DrawerStrategyMenuItem dsmi in renderingModeMenuItem.DropDownItems)
            {
                dsmi.Checked = (dsmi.Strategy == CurrentDrawerStrategy);
            }
        }

        private void gridSnappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_gridSnapDialog == null)
            {
                _gridSnapDialog = new GridSnapDialog(this);
                _gridSnapDialog.FormClosed += delegate
                {
                    _gridSnapDialog = null;
                };
            }
            _gridSnapDialog.Show();
        }

        private void snapToGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewportControl.Gizmo.SnapEnabled = SnapEnabled;
        }

        private void gizmoSpaceToolStripButton_Click(object sender, EventArgs e)
        {
            viewportControl.Gizmo.ToggleActiveSpace();
        }
        #endregion

        #region Edit & View menu items

        private void duplicateMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectionManager.Empty) return;
            Logic.WrappedScene.Duplicate(SelectionManager.CurrentSelection[0]);
        }


        private void reloadMenuItem_Click(object sender, EventArgs e)
        {
            OpenScene(Logic.SceneFile, false);
            Invalidate();
        }

        private void zeroTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForwardTimeAndUpdate(0);
        }

        private void forward01sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForwardTimeAndUpdate(0.1);
        }

        private void ForwardTimeAndUpdate(double seconds)
        {
            Logic.ForwardTime(seconds);
            Logic.WrappedScene.Update();
            Invalidate(true);
            Logic.FinishedUpdate();
        }
        #endregion

        #region CollisionTree
        private void CollisionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CollisionTreeNode node = e.Node as CollisionTreeNode;
            ContextMenuStrip popup = new ContextMenuStrip();
            if (node == null || node.WrappedCollider == null)
            {
                if (collisionTreeView.Collision == null) return;

                popup.Items.Add("Add Sphere collider").Click += delegate
                {
                    collisionTreeView.Collision.AddSphere();
                };
                popup.Items.Add("Add Box collider").Click += delegate
                {
                    collisionTreeView.Collision.AddBox();
                };
                collisionTreeView.ContextMenuStrip = popup;
                return;
            }
            propertyGrid.SelectedObject = node.WrappedCollider;
            propertyGrid.ExpandAllGridItems();


            popup.Items.Add("Remove").Click += delegate
            {
                collisionTreeView.Collision.Remove(node.WrappedCollider);
                ReSelectSceneNode();
            };
            collisionTreeView.ContextMenuStrip = popup;
        }

        public void ReSelectSceneNode()
        {
            SceneTreeView_AfterSelect(this, new TreeViewEventArgs(sceneTreeView.SelectedNode));
        }


        private void CollisionTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                collisionTreeView.SelectedNode = e.Node;
            }
            else if (e.Button == MouseButtons)
                CollisionTreeView_AfterSelect(sender, new TreeViewEventArgs(e.Node, TreeViewAction.ByMouse));
        }

        private void collisionTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (collisionTreeView.HitTest(e.Location).Node == null)
            {
                collisionTreeView.SelectedNode = null;
                SceneTreeNode node = sceneTreeView.SelectedNode as SceneTreeNode;
                if (node != null)
                {
                    propertyGrid.SelectedObject = node.WrappedGameObject;
                    propertyGrid.ExpandAllGridItems();
                }
            }
        }

        #endregion
        #endregion


        #region Materials
        private void GenerateMaterials()
        {
            Effect effect = Logic.ResourceManager.ShaderEffects.First(shaderEffect => shaderEffect.Name == @"Effects\BasicShader");
            HashSet<string> textures = new HashSet<string>();

            foreach (Texture2D tex in Logic.ResourceManager.Textures)
            {
                textures.Add(tex.Name);
            }

            Texture2D defaultNormal   = Logic.ResourceManager.GetTexture(@"Textures\Default_n");
            Texture2D defaultSpecular = Logic.ResourceManager.GetTexture(@"Textures\Default_s");
            textures.Remove(@"Textures\Default_n");
            textures.Remove(@"Textures\Default_s");

            foreach (MeshMaterial mat in Logic.ResourceManager.Materials)
            {
                if (mat.Diffuse  != null) textures.Remove(mat.Diffuse.Name);
                if (mat.Normal   != null) textures.Remove(mat.Normal.Name);
                if (mat.Specular != null) textures.Remove(mat.Specular.Name);
                if (mat.Emissive != null) textures.Remove(mat.Emissive.Name);
            }

            int id = Logic.ResourceManager.Materials.Select(material => material.Id).Max();
            List<string> rejectedNoDiffuse = new List<string>();
            List<string> rejectedNoSpecial = new List<string>();

            while (textures.Count != 0)
            {
                string candidate = textures.First();
                string[] splitted = candidate.Split('_');
                string suffix = splitted.Last();
                string name = candidate.Substring(0, candidate.Length - suffix.Length - 1);

                Texture2D diffuse  = Logic.ResourceManager.GetTexture(name + "_d");
                Texture2D normal   = Logic.ResourceManager.GetTexture(name + "_n") ?? defaultNormal;
                Texture2D specular = Logic.ResourceManager.GetTexture(name + "_s") ?? defaultSpecular;
                Texture2D emissive = Logic.ResourceManager.GetTexture(name + "_e") ?? diffuse;

                if (diffuse == null)
                {
                    rejectedNoDiffuse.Add(candidate);
                    Logic.Logger.Log(LoggerLevel.Warning, "Not found _d texture for " + name);
                    textures.Remove(candidate);
                }
                else
                {
                    if (normal == defaultNormal || specular == defaultSpecular)
                    {
                        rejectedNoSpecial.Add(name);
                        Logic.Logger.Log(LoggerLevel.Warning, "Used default normal or specular texture for " + name);
                    }

                    const string prefix = @"Textures\";
                    if (name.StartsWith(prefix)) name = name.Substring(prefix.Length);
                    MeshMaterial material = new MeshMaterial(id, name, diffuse, normal, specular, emissive, effect);
                    Logic.ResourceManager.Materials.Add(material);
                    id++;
                    textures.Remove(diffuse.Name);

                    if (normal   != defaultNormal)   textures.Remove(normal.Name);
                    if (specular != defaultSpecular) textures.Remove(specular.Name);
                    if (emissive != null) textures.Remove(emissive.Name);
                }
            }

            StringBuilder sb = new StringBuilder();
            if (rejectedNoDiffuse.Count > 0)
            {
                sb.AppendLine("No diffuse found for:");
                foreach (string tex in rejectedNoDiffuse)
                {
                    sb.AppendLine(tex);
                }
                sb.AppendLine();
            }

            if (rejectedNoSpecial.Count > 0) { 
                sb.AppendLine("Used default normal/specular for:");
                foreach (string tex in rejectedNoSpecial)
                {
                    sb.AppendLine(tex);
                }
            }
            if (sb.Length > 0)
                MessageBox.Show(sb.ToString(), "Not all textures used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void generateMaterialsMenuItem_Click(object sender, EventArgs e)
        {
            GenerateMaterials();
            SaveContent();
            OpenContent(Logic.ContentFile); // lame way of reloading content
        }
        #endregion

        public class DrawerStrategyMenuItem : ToolStripMenuItem
        {
            public IDrawerStrategy Strategy { get; private set; }

            public DrawerStrategyMenuItem(string text, IDrawerStrategy strategy) : base(text)
            {
                Strategy = strategy;
            }
        }


        public void SelectGameObject(GameObjectWrapper collider)
        {
            sceneTreeView.SelectedNode = collider.TreeViewNode;
        }

        public void RefreshPropertyGrid()
        {
            propertyGrid.Refresh();
        }

        #endregion

    }
}
