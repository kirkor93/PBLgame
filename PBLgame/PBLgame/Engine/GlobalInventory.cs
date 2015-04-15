using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine
{
    /// <summary>
    /// Ugly static class that contains a few useful shortcut (tunnel under classes) references for whole engine, game and editor.
    /// </summary>
    public class GlobalInventory
    {
        #region Variables
        #endregion

        #region Properties
        public static GlobalInventory Instance { get; private set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        #endregion

        private GlobalInventory() { }

        static GlobalInventory()
        {
            Instance = new GlobalInventory();
        }
    }
}