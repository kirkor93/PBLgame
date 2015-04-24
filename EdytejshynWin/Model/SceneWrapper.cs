using System;
using System.Collections.Generic;
using Edytejshyn.GUI;
using Edytejshyn.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;

namespace Edytejshyn.Model
{
    /// <summary>
    /// Wrapper for the game scene. Contains GameObjectWrappers and works as a bridge between Wrappers and SceneTreeNode.
    /// </summary>
    public class SceneWrapper
    {
        public List<GameObjectWrapper> RootGameObjects { get; private set; }
        public SceneTreeView TreeView { get; set; }
        public EditorLogic Logic { get; private set; }
        public Scene Scene { get; private set; }

        public IEnumerable<GameObjectWrapper> AllGameObjects
        {
            get
            {
                foreach (GameObjectWrapper root in RootGameObjects)
                {
                    yield return root;
                    foreach (GameObjectWrapper child in root.Descendants)
                    {
                        yield return child;
                    }
                }
            }
        }

        #region Methods

        public SceneWrapper(EditorLogic logic, Scene scene)
        {
            Logic = logic;
            Scene = scene;
            RootGameObjects = new List<GameObjectWrapper>();
            
            foreach (GameObject obj in Scene.GameObjects)
            {
                if (obj.parent != null) continue;
                RootGameObjects.Add(WrapWithHandler(obj, null));
            }

            foreach (Light light in Scene.SceneLights)
            {
                if (light.parent != null) continue;
                RootGameObjects.Add(WrapWithHandler(light, null));
            }
        }

        private GameObjectWrapper WrapWithHandler(GameObject obj, GameObjectWrapper parent)
        {
            GameObjectWrapper wrapper = GameObjectWrappingFactory.Wrap(obj, parent);
            wrapper.AttachSetterHandler(delegate(ICommand command)
            {
                Logic.History.NewAction(command);
            });
            return wrapper;
        }

        public void Draw(IDrawerStrategy strategy, GameTime gameTime)
        {
            // TODO remove recurency in children Draw() - use enumerator
            foreach (GameObjectWrapper wrapper in RootGameObjects)
            {
                wrapper.Draw(strategy, gameTime);
            }
        }

        public GameObjectWrapper ClosestIntersector(Ray ray)
        {
            // TODO optimize in list/map of BSpheres
            GameObjectWrapper closest = null;
            float closestDistance = float.MaxValue;
            foreach (GameObjectWrapper wrapper in AllGameObjects)
            {
                if(wrapper.Renderer == null) continue;
                ModelMeshCollection modelMeshes = wrapper.Renderer.Mesh.Model.Meshes;
                
                int id = 0;
                for(int i = 0; i < modelMeshes.Count; i++)
                {
                    var mesh = modelMeshes[i];
                    float? dist = ray.Intersects(mesh.BoundingSphere.Transform(wrapper.Nut.transform.World));
                    if ( (dist.HasValue)  &&  (dist < closestDistance) )
                    {
                        closest = wrapper;
                        closestDistance = dist.Value;
                        id = i;
                    }
                }
            }
            Console.WriteLine("Intersect: {0}: {1}", (closest == null) ? "none" : closest.Name, closestDistance);
            return closest;
        }

        /// <summary>
        /// Duplicates given GameObject inside Wrapper and places in the same parent.
        /// This action can be undone with HistoryManager.
        /// </summary>
        /// <param name="source">GamObjectWrapper with GameObject to duplicate</param>
        public void Duplicate(GameObjectWrapper source)
        {
            GameObjectWrapper copy = GameObjectWrappingFactory.Copy(source, source.Parent, Scene);

            Logic.History.NewAction(new AddGameObjectCommand(this, copy, source.Parent));
        }
        #endregion

        /// <summary>
        /// Kidnaps given GameObject from its parent and leaves him with newParent.
        /// This action can be undone with HistoryManager.
        /// </summary>
        /// <param name="kidnapped">GameObject to kidnap</param>
        /// <param name="newParent">new parent</param>
        public void ReparentNode(GameObjectWrapper kidnapped, GameObjectWrapper newParent)
        {
            if (kidnapped.Parent == newParent) return;

            Logic.History.NewAction(new ReparentGameObjectCommand(this, kidnapped, newParent));

        }
    }

    public class AddGameObjectCommand : ICommand
    {
        private readonly SceneWrapper _sceneWrapper;
        private readonly GameObjectWrapper _addee;
        private readonly GameObjectWrapper _parent;
        private readonly SelectionManager.Memento _oldSelection;

        public AddGameObjectCommand(SceneWrapper sceneWrapper, GameObjectWrapper addee, GameObjectWrapper parent)
        {
            _sceneWrapper = sceneWrapper;
            _addee = addee;
            _parent = parent;
            _oldSelection = _sceneWrapper.Logic.SelectionManager.CreateMemento();
        }

        public bool AffectsData { get { return true; } }

        public string Description
        {
            get { return string.Format("Add game object: {0}", _addee.Name); }
        }

        public string Message
        {
            get { return string.Format("Added: [{0}] {1} into {2}", _addee.Nut.ID, _addee.Name, GameObjectWrapper.GetFullPathString(_addee.Parent)); }
        }

        public void Do()
        {
            _sceneWrapper.Scene.AddGameObjectWithDescendants(_addee.Nut);
            if (_parent == null)
            {
                _sceneWrapper.RootGameObjects.Add(_addee);
                _sceneWrapper.TreeView.Nodes.Add(new SceneTreeNode(_addee));
            }
            else
            {
                _parent.Children.Add(_addee);
                _parent.TreeViewNode.Nodes.Add(new SceneTreeNode(_addee));
            }

            _sceneWrapper.Logic.SelectionManager.SelectOnly(_addee);
        }

        public void Undo()
        {
            if (_parent == null)
            {
                _sceneWrapper.RootGameObjects.Remove(_addee);
                _sceneWrapper.TreeView.Nodes.Remove(_addee.TreeViewNode);
            }
            else
            {
                _parent.Children.Remove(_addee);
                _parent.TreeViewNode.Nodes.Remove(_addee.TreeViewNode);
            }
            _sceneWrapper.Scene.RemoveGameObjectWithDescendants(_addee.Nut);
            _sceneWrapper.Logic.SelectionManager.ApplyMemento(_oldSelection);
        }
    }

    public class ReparentGameObjectCommand : ICommand
    {
        private readonly SceneWrapper _sceneWrapper;
        private readonly GameObjectWrapper _kidnapped;
        private readonly GameObjectWrapper _oldParent;
        private readonly SelectionManager.Memento _oldSelection;
        private readonly TransformWrapper.Memento _oldTransform;

        private readonly GameObjectWrapper _newParent;
        private readonly string _oldPathString;

        public ReparentGameObjectCommand(SceneWrapper sceneWrapper, GameObjectWrapper kidnapped, GameObjectWrapper newParent)
        {
            _sceneWrapper = sceneWrapper;
            _kidnapped = kidnapped;
            _newParent = newParent;
            _oldParent = kidnapped.Parent;
            _oldTransform = kidnapped.Transform.CreateMemento();
            _oldSelection = _sceneWrapper.Logic.SelectionManager.CreateMemento();
            _oldPathString = GameObjectWrapper.GetFullPathString(_kidnapped);
        }

        public bool AffectsData { get { return true; } }

        public string Description
        {
            get { return string.Format("Kidnapped game object: {0}", _kidnapped.Name); }
        }

        public string Message
        {
            get { return string.Format("Kidnapped: [{0}] {1} into {2}", _kidnapped.Nut.ID, _oldPathString, GameObjectWrapper.GetFullPathString(_newParent)); }
        }

        public void Do()
        {
            // TODO ***************************
            // TODO *                         *
            // TODO *  recalculate transform  *
            // TODO *          here           *
            // TODO *      if you can         *
            // TODO *   but they say you      *
            // TODO *     don't know how      *
            // TODO *                         *
            // TODO ***************************
            
            ApplyKidnapping(_newParent);
            _sceneWrapper.Logic.SelectionManager.SelectOnly(_kidnapped);
        }

        public void Undo()
        {
            ApplyKidnapping(_oldParent);
            _kidnapped.Transform.ApplyMemento(_oldTransform);
            _sceneWrapper.Logic.SelectionManager.ApplyMemento(_oldSelection);
        }

        private void ApplyKidnapping(GameObjectWrapper newParent)
        {
            // FIXME do clean code - that redistribution looks bad and illegible
            _kidnapped.Reparent(newParent);

            if (_kidnapped.Parent == null)
            {
                _sceneWrapper.RootGameObjects.Remove(_kidnapped);
                _sceneWrapper.TreeView.Nodes.Remove(_kidnapped.TreeViewNode);
            }
            else
            {
                _kidnapped.Parent.TreeViewNode.Nodes.Remove(_kidnapped.TreeViewNode);
            }
            if (newParent == null)
            {
                _sceneWrapper.RootGameObjects.Add(_kidnapped);
                _sceneWrapper.TreeView.Nodes.Add(new SceneTreeNode(_kidnapped)); // without creating new treeNode every time, something crashes when redoing
            }
            else
            {
                newParent.TreeViewNode.Nodes.Add(new SceneTreeNode(_kidnapped));
            }

        }
    }
}