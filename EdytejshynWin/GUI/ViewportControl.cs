using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Edytejshyn.GUI.XNA;
using Edytejshyn.Logic;
using Edytejshyn.Logic.Commands;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using Color = Microsoft.Xna.Framework.Color;
using Keys = System.Windows.Forms.Keys;

namespace Edytejshyn.GUI
{
    public class ViewportControl : GraphicsDeviceControl
    {

        #region Variables
        private ContentManager _editorContent;
        private SpriteBatch _spriteBatch;
        private SpriteFont _osdFont;
        private EditorMouseState _currentMouse, _prevMouse;
        private Timer _timer;
        private int _moveX, _moveY;
        private const float BASE_ROTATE_SENSITIVITY = 0.004f, BASE_MOVE_SENSITIVITY = 0.15f;
        private float _rotateSensitivity = BASE_ROTATE_SENSITIVITY, _moveSensitivity = BASE_MOVE_SENSITIVITY;
        private CameraCommand _cameraCommand;

        public delegate void VoidHandler();
        public event VoidHandler AfterInitializeEvent = () => { };

        public Camera Camera { get; private set; }
        public Grid Grid { get; private set; }

        //public GameObject SampleObject;
        public MainForm MainForm;

        #endregion

        #region Properties
        public ContentManager GameContentManager { get; private set; }

        #endregion

        #region Methods
        protected override void Initialize()
        {
            _timer = new Timer();
            _timer.Interval = 15;
            _timer.Tick += TimerOnTick;
            _timer.Start();

            this.MouseMove += ViewportControl_MouseMove;
            this.MouseDown += ViewportControl_MouseDown;
            this.MouseUp   += ViewportControl_MouseUp;

            this.PreviewKeyDown += ViewportControl_PreviewKeyDown;
            this.KeyDown        += ViewportControl_KeyDown;
            this.KeyUp          += ViewportControl_KeyUp;
            this.SizeChanged += HandleSizeChanged;
            
            _currentMouse = new EditorMouseState();
            _prevMouse    = new EditorMouseState();
            
            _editorContent = new ContentManager(Services, "EditorContent");
            GameContentManager   = new ContentManager(Services, "Content");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _osdFont = _editorContent.Load<SpriteFont>("OSDFont");
            Camera = new Camera(new Vector3(0, 10, 10), Vector3.Zero, Vector3.Up, MathHelper.PiOver4, ClientSize.Width, ClientSize.Height, 1, 1000);
            Grid = new Grid(this, 2, 100);
            Reset();
            
            AfterInitializeEvent();
            this.MainForm.Logic.History.UpdateEvent += delegate(HistoryManager manager)
            {
                Invalidate();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _editorContent.Unload();
                GameContentManager.Unload();
            }
            base.Dispose(disposing);
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Scene scene = MainForm.Logic.CurrentScene;
            if (scene != null)
            {
                // wrong stencil fix:
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                //GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                MainForm.Logic.CurrentScene.Draw();
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Grid.Draw();

            _spriteBatch.Begin();
            Vector2 position = new Vector2(5, 5);
            _spriteBatch.DrawString(_osdFont, Text, position, Color.White, 0.0f, Vector2.Zero, new Vector2(0.75f), SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        /// <summary>
        /// Resets / reverts viewport to initial state.
        /// Camera moved to default location.
        /// </summary>
        public void Reset()
        {
            Camera.transform.Position = new Vector3(0, 10, 10);
            Camera.SetTarget(new Vector3(0, 0, 0));
            Camera.Update();
        }

        #region Event handlers

        protected void ViewportControl_MouseMove(object sender, MouseEventArgs e)
        {
            _prevMouse = _currentMouse;
            _currentMouse = new EditorMouseState(_prevMouse) {X = e.X, Y = e.Y};
            if (_currentMouse.AnyMiddle)
            {
                // surface strafe
            }
            else if (_currentMouse.Left)
            {
                // if(!gizmo_collision)
                // find rayed object
                Vector3 nearVector = new Vector3(_currentMouse.Vector, 0f);
                Vector3 farVector = new Vector3(_currentMouse.Vector, 1f);
                Vector3 nearUnproj = GraphicsDevice.Viewport.Unproject(nearVector, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
                Vector3 farUnproj = GraphicsDevice.Viewport.Unproject(farVector, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
                Vector3 direction = farUnproj - nearUnproj;
                direction.Normalize();
                Ray picker = new Ray(nearUnproj, direction);

                // TODO collision with scene objects
                //var modelMeshes = SampleObject.renderer.MyMesh.Model.Meshes;
                //float? distance = null;
                //int id = 0;
                //for(int i = 0; i < modelMeshes.Count; i++)
                //{
                //    var mesh = modelMeshes[i];
                //    float? d = picker.Intersects(mesh.BoundingSphere.Transform(SampleObject.transform.World));
                //    if ( (d.HasValue)  &&  (distance == null || d > distance) )
                //    {
                //        distance = d;
                //        id = i;
                //    }
                //}
                //Text = distance.HasValue ? string.Format("Collision: [{1}] {0}\ndist = {2}", modelMeshes[id].Name, id, distance) : string.Format("Ray: origin {0}\ndir {1}", nearUnproj, direction);

            }
            else if (_currentMouse.Right)
            {
                Vector2 rot = (_prevMouse.Vector - _currentMouse.Vector) * _rotateSensitivity;
                Camera.RotateYawPitch(rot.X, rot.Y);
                Text = string.Format("Lookaround: {0}, {1}", rot.X, rot.Y);
                Camera.Update();
            }
            else
            {
                // TODO search collision with gizmo
                Text = string.Format("-");
            }
            Invalidate();
        }

        protected void ViewportControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.Select();
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

                    _cameraCommand = new CameraCommand(Camera);
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
                    _cameraCommand.SaveFinalState();
                    this.MainForm.Logic.History.NewAction(_cameraCommand);
                    _cameraCommand = null;
                    break;
            }
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (!_currentMouse.Right) return;
            if (_moveX != 0)
            {
                Vector3 cameraStrafe = Vector3.Cross(Camera.Direction, Vector3.Up);
                cameraStrafe.Normalize();
                Camera.transform.Position += cameraStrafe * _moveX * _moveSensitivity;
            }
            
            if (_moveY != 0)
            {
                Camera.transform.Position += Camera.Direction * _moveY * _moveSensitivity;
            }

            if (_moveX != 0 || _moveY != 0)
            {
                Camera.Update();
                Invalidate();
            }
        }

        private void ViewportControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Allows to catch arrow keys and hotkey combinations like Ctrl+S,
            // which are swallowed by MainForm otherwise.
            if (!_currentMouse.Right) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                case Keys.ControlKey:
                case Keys.ShiftKey:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void ViewportControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_currentMouse.Right) return;
            
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    _moveY = 1;
                    break;

                case Keys.Down:
                case Keys.S:
                    _moveY = -1;
                    break;

                case Keys.Left:
                case Keys.A:
                    _moveX = -1;
                    break;

                case Keys.Right:
                case Keys.D:
                    _moveX = 1;
                    break;

                case Keys.ShiftKey:
                    _moveSensitivity = BASE_MOVE_SENSITIVITY * 10f;
                    break;

                case Keys.ControlKey:
                    _moveSensitivity   = BASE_MOVE_SENSITIVITY * 0.1f;
                    _rotateSensitivity = BASE_ROTATE_SENSITIVITY * 0.1f;
                    break;
            }

        }


        private void ViewportControl_KeyUp(object sender, KeyEventArgs e)
        {
            //if (!_currentMouse.Right) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                case Keys.Down:
                case Keys.S:
                    _moveY = 0;
                    break;

                case Keys.Left:
                case Keys.A:
                case Keys.Right:
                case Keys.D:
                    _moveX = 0;
                    break;

                case Keys.ShiftKey:
                    _moveSensitivity = BASE_MOVE_SENSITIVITY;
                    break;

                case Keys.ControlKey:
                    _moveSensitivity   = BASE_MOVE_SENSITIVITY;
                    _rotateSensitivity = BASE_ROTATE_SENSITIVITY;
                    break;
            }
        }

        protected void HandleSizeChanged(object sender, EventArgs e)
        {
            Camera.SetAspect(ClientSize.Width, ClientSize.Height);
            Invalidate();
        }

        #endregion
        #endregion
    }
}