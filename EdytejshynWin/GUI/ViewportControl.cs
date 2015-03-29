using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Edytejshyn.GUI
{
    public class ViewportControl : GraphicsDeviceControl
    {

        #region Variables
        private ContentManager _editorContent, _gameContent;
        private SpriteBatch _spriteBatch;
        private SpriteFont _osdFont;

        public delegate void VoidHandler();
        public event VoidHandler AfterInitializeEvent = () => { };
        #endregion

        #region Properties
        public ContentManager GameContent
        {
            get { return _gameContent; }
        }

        #endregion

        #region Methods
        protected override void Initialize()
        {
            this.MouseMove += ViewportControl_MouseMove;
            _editorContent = new ContentManager(Services, "EditorContent");
            _gameContent   = new ContentManager(Services, "Content");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _osdFont = _editorContent.Load<SpriteFont>("OSDFont");
            AfterInitializeEvent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _editorContent.Unload();
                _gameContent.Unload();
            }
            base.Dispose(disposing);
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_osdFont, Text, new Vector2(2, 2), Color.White);
            _spriteBatch.End();
        }

        public void ViewportControl_MouseMove(object sender, MouseEventArgs e)
        {
            Text = string.Format("{0}, {1}", e.X, e.Y);
            Invalidate();
        }
        #endregion
    }
}