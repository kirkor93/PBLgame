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

        public void Duplicate(GameObjectWrapper source)
        {
            GameObjectWrapper copy = GameObjectWrappingFactory.Copy(source, source.Parent, Scene);

            Logic.History.NewAction(new AddGameObjectCommand(this, copy, source.Parent));
        }
        #endregion
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
            get { return string.Format("Added: [{0}] {1} into {2}", _addee.Nut.ID, _addee.Name, _addee.GetPathString()); }
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
}