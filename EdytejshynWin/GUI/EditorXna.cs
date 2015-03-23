using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Edytejshyn.GUI
{
    public class EditorXna : Game
    {
        #region Variables
        private readonly GraphicsDeviceManager graphics;
        private MainForm form;
        #endregion

        #region Methods
        public EditorXna(MainForm form)
        {
            this.form = form;
            form.XnaGame = this;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth  = form.RenderWindow.Width,
                PreferredBackBufferHeight = form.RenderWindow.Height
            };

            graphics.PreparingDeviceSettings += (sender, e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = form.RenderWindow.Handle;
            };

            Control.FromHandle(Window.Handle).VisibleChanged += (sender, args) =>
            {
                Control window = Control.FromHandle(Window.Handle);
                if (window.Visible)
                    window.Visible = false;
            };

            IsMouseVisible = true;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion
    }
}