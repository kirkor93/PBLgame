using System.Collections.Generic;
using Edytejshyn.Logic;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Model
{
    public class SceneWrapper
    {
        private EditorLogic _logic;
        public List<GameObjectWrapper> GameObjects { get; private set; }

        public SceneWrapper(EditorLogic logic)
        {
            _logic = logic;
            GameObjects = new List<GameObjectWrapper>();
            
            foreach (GameObject obj in _logic.CurrentScene.GameObjects)
            {
                if (obj.parent != null) continue;
                GameObjects.Add(WrapWithHandler(obj));
            }

            foreach (Light light in _logic.CurrentScene.SceneLights)
            {
                if (light.parent != null) continue;
                GameObjects.Add(WrapWithHandler(light));
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

        public void Draw(IDrawerStrategy strategy)
        {
            foreach (GameObjectWrapper wrapper in GameObjects)
            {
                wrapper.Draw(strategy);
            }
        }

    }

}