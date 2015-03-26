using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Edytejshyn.GUI
{
    public class ViewportControl : GraphicsDeviceControl
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _osdFont;

        public ViewportControl()
        {
            this.MouseMove += ViewportControl_MouseMove;
        }

        protected override void Initialize()
        {
            _content = new ContentManager(Services, "EditorContent");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _osdFont = _content.Load<SpriteFont>("OSDFont");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Unload();
            }
            base.Dispose(disposing);
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_osdFont, Text, new Vector2(2, 2), Color.Black);
            _spriteBatch.End();
        }

        public void ViewportControl_MouseMove(object sender, MouseEventArgs e)
        {
            Text = string.Format("{0}, {1}", e.X, e.Y);
            Invalidate();
        }
    }
}