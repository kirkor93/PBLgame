using System.Collections.Generic;
using Edytejshyn.GUI;
using Edytejshyn.Model;

namespace Edytejshyn.Logic
{
    /// <summary>
    /// Selection manager for handling multiple selected objects.
    /// </summary>
    public class SelectionManager
    {
        // TODO history handling
        private EditorLogic _logic;
        private SceneTreeView _treeView;
        private ViewportControl _viewport;
        public bool Empty { get { return _selection.Count == 0; } }

        private readonly List<GameObjectWrapper> _selection = new List<GameObjectWrapper>();

        public SelectionManager(EditorLogic logic, SceneTreeView treeView, ViewportControl viewport)
        {
            _logic = logic;
            _treeView = treeView;
            _viewport = viewport;
        }

        /// <summary>
        /// Adds to selection.
        /// </summary>
        /// <param name="selecting">Object to append to selection</param>
        public void Append(GameObjectWrapper selecting)
        {
            _selection.Add(selecting);
            _treeView.SelectedNode = selecting.TreeViewNode;
            // TODO handle colouring in TreeView
        }

        /// <summary>
        /// Removes from selection (deselects).
        /// </summary>
        /// <param name="deselecting">Object to deselect</param>
        /// <returns>whether found and deselected</returns>
        public bool Remove(GameObjectWrapper deselecting)
        {
            return _selection.Remove(deselecting);
        }

        /// <summary>
        /// Selects only one game object. Others are deselected.
        /// Null can be given to deselect all.
        /// </summary>
        /// <param name="selecting">Object to select</param>
        /// <param name="selectTreeView">Set this to false if calling from TreeView after selection.</param>
        public void SelectOnly(GameObjectWrapper selecting, bool selectTreeView = true)
        {
            _selection.Clear();
            if (selecting == null) return;
            _selection.Add(selecting);
            if(selectTreeView) _treeView.SelectedNode = selecting.TreeViewNode;
            _viewport.Invalidate();
        }

        public GameObjectWrapper[] CurrentSelection
        {
            get { return _selection.ToArray(); }
        }

        /// <summary>
        /// Get effective selection including descendants of selected parent nodes.
        /// Behaviour known in Unity where descendants are highlighted in viewport.
        /// </summary>
        public List<GameObjectWrapper> SelectionWithDescendants
        {
            get
            {
                var tmpList = new List<GameObjectWrapper>(_selection);
                foreach (GameObjectWrapper selected in _selection)
                {
                    foreach (GameObjectWrapper descendant in selected.Descendants)
                    {
                        tmpList.Add(descendant);
                    }
                }
                return tmpList;
            }
        }

        public Memento CreateMemento()
        {
            return new Memento(CurrentSelection);
        }

        public void ApplyMemento(Memento memento)
        {
            // TODO handling multiselection
            GameObjectWrapper selectee = (memento.GetState().Length == 0) ? null : memento.GetState()[0];
            SelectOnly(selectee);
        }

        public class Memento
        {
            private readonly GameObjectWrapper[] _selection;

            public Memento(GameObjectWrapper[] selection)
            {
                _selection = selection;
            }

            public GameObjectWrapper[] GetState()
            {
                return _selection;
            }
        }
    }
}