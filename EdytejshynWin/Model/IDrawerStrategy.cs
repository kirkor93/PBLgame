using Microsoft.Xna.Framework;

namespace Edytejshyn.Model
{
    public interface IDrawerStrategy
    {
        void Draw(GameObjectWrapper gameObjectWrapper, GameTime gameTime);
        bool IsRealistic { get; }
    }

}