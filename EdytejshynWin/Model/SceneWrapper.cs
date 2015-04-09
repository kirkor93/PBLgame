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
                GameObjectWrapper wrapper = new GameObjectWrapper(obj);
                wrapper.AttachSetterHandler(delegate(ICommand command)
                {
                    _logic.History.NewAction(command);
                });
                GameObjects.Add(wrapper);
            }
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