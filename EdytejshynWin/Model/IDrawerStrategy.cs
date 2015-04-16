using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;

namespace Edytejshyn.Model
{
    public interface IDrawerStrategy
    {
        void Draw(GameObjectWrapper gameObjectWrapper, GameTime gameTime);
    }

}