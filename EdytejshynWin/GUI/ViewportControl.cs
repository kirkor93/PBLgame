using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    public class ViewportControl : GraphicsDeviceControl
    {

        #region Variables
        private ContentManager _editorContent, _gameContent;
        private SpriteBatch _spriteBatch;
        private SpriteFont _osdFont;
        private EditorMouseState _currentMouse, _prevMouse;

        public delegate void VoidHandler();
        public event VoidHandler AfterInitializeEvent = () => { };

        public Camera Camera { get; private set; }

        public GameObject SampleObject;

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
            this.MouseDown += ViewportControl_MouseDown;
            this.MouseUp   += ViewportControl_MouseUp;
            this.SizeChanged += HandleSizeChanged;
            _currentMouse = new EditorMouseState();
            _prevMouse    = new EditorMouseState();

            _editorContent = new ContentManager(Services, "EditorContent");
            _gameContent   = new ContentManager(Services, "Content");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _osdFont = _editorContent.Load<SpriteFont>("OSDFont");
            Camera = new Camera(new Vector3(0, 10, 10), Vector3.Zero, Vector3.Up, MathHelper.PiOver4, ClientSize.Width, ClientSize.Height, 1, 1000);

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

            if (SampleObject != null)
            {
                SampleObject.renderer.Draw();
            }

            _spriteBatch.Begin();
            Vector2 position = new Vector2(5, 5);
            _spriteBatch.DrawString(_osdFont, Text, position, Color.White, 0.0f, Vector2.Zero, new Vector2(0.75f), SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        protected void ViewportControl_MouseMove(object sender, MouseEventArgs e)
        {
            _prevMouse = _currentMouse;
            _currentMouse = new EditorMouseState(_prevMouse) {Position = {X = e.X, Y = e.Y}};
            if (_currentMouse.Right)
            {
                Vector2 diff = _prevMouse.Position - _currentMouse.Position;
                const float sensitivity = 0.01f;
                Camera.Direction += new Vector3(diff.X*sensitivity, diff.Y*sensitivity, 0f);
                Text = string.Format("{0}, {1}", diff.X, diff.Y);
                Camera.Update(null);
            }
            else
            {
                Text = string.Format("-");
            }
            Invalidate();
        }

        protected void ViewportControl_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // select objects on scene / interact with gizmo
                    _currentMouse.Left = true;
                    break;
                case MouseButtons.Middle:
                    // camera strafe movement
                    _currentMouse.Middle = true;
                    break;
                case MouseButtons.Right:
                    // FPS camera lookaround
                    _currentMouse.Right = true;
                    Cursor.Current = Cursors.SizeAll;
                    break;
            }
        }

        protected void ViewportControl_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _currentMouse.Left = false;
                    break;
                case MouseButtons.Middle:
                    _currentMouse.Middle = false;
                    break;
                case MouseButtons.Right:
                    _currentMouse.Right = false;
                    Cursor.Current = Cursors.Default;
                    break;
            }
        }

        protected void HandleSizeChanged(object sender, EventArgs e)
        {
            Camera.SetAspect(ClientSize.Width, ClientSize.Height);
            Invalidate();
        }

       
        
        #endregion
    }
}