using System;
using System.Collections.Generic;
using Edytejshyn.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Model
{
    public class SceneWrapper
    {
        private EditorLogic _logic;
        public List<GameObjectWrapper> RootGameObjects { get; private set; }

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


        public SceneWrapper(EditorLogic logic)
        {
            _logic = logic;
            RootGameObjects = new List<GameObjectWrapper>();
            
            foreach (GameObject obj in _logic.CurrentScene.GameObjects)
            {
                if (obj.parent != null) continue;
                RootGameObjects.Add(WrapWithHandler(obj));
            }

            foreach (Light light in _logic.CurrentScene.SceneLights)
            {
                if (light.parent != null) continue;
                RootGameObjects.Add(WrapWithHandler(light));
            }
        }

        private GameObjectWrapper WrapWithHandler(GameObject obj)
        {
            GameObjectWrapper wrapper = GameObjectWrappingFactory.Wrap(obj);
            wrapper.AttachSetterHandler(delegate(ICommand command)
            {
                _logic.History.NewAction(command);
            });
            return wrapper;
        }

        public void Draw(IDrawerStrategy strategy, GameTime gameTime)
        {
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
            // TODO recursive
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
    }

}