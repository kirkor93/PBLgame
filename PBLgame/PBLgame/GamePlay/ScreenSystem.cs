using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    public abstract class ScreenSystem
    {
        public abstract void Draw(SpriteBatch batch);
        public abstract void Update(GameTime gameTime);
    }
}
