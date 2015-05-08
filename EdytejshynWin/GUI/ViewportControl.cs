using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Edytejshyn.GUI.XNA;
using Edytejshyn.Logic;
using Edytejshyn.Model;
using PBLgame.Engine;
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
        private const float BASE_ROTATE_SENSITIVITY = 0.006f, BASE_MOVE_SENSITIVITY = 1.5f;
        private float _rotateSensitivity = BASE_ROTATE_SENSITIVITY, _moveSensitivity = BASE_MOVE_SENSITIVITY;
        private bool _hasFocus;

        public delegate void VoidHandler();
        public event VoidHandler AfterInitializeEvent = () => { };

        public MainForm MainForm;
        private bool _mouseMoved;
        private int _counter;
        Stopwatch _stopWatch;
        private long _lastElapsed;

        #endregion

        #region Properties
        public ContentManager GameContentManager { get; private set; }
        public CameraHistory CameraHistory { get; private set; }
        public Camera Camera { get; private set; }
        public Grid Grid { get; private set; }
        public Gizmo Gizmo { get; set; }
        #endregion

        #region Methods
        protected override void Initialize()
        {
            this.MouseMove += ViewportControl_MouseMove;
            this.MouseDown += ViewportControl_MouseDown;
            this.MouseUp   += ViewportControl_MouseUp;

            this.PreviewKeyDown += ViewportControl_PreviewKeyDown;
            this.KeyDown        += ViewportControl_KeyDown;
            this.KeyUp          += ViewportControl_KeyUp;
            this.SizeChanged += HandleSizeChanged;

            this.Enter += delegate
            {
                _hasFocus = true;
                Text = "-";
                Invalidate();
            };
            this.Leave += delegate
            {
                _hasFocus = false;
                Invalidate();
            };

            _currentMouse = new EditorMouseState();
            _prevMouse    = new EditorMouseState();

            GlobalInventory.Instance.GraphicsDevice = GraphicsDevice;
            _editorContent     = new ContentManager(Services, "EditorContent");
            GameContentManager = new ContentManager(Services, "Content");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _osdFont = _editorContent.Load<SpriteFont>("OSDFont");
            Camera = new Camera(new Vector3(0, 10, 10), Vector3.Zero, Vector3.Up, MathHelper.PiOver4, ClientSize.Width, ClientSize.Height, 1, 10000);
            CameraHistory = new CameraHistory(MainForm.Logic.Logger, Camera);
            Grid = new Grid(this, 1, 100);
            Gizmo = new Gizmo(this, _spriteBatch, _editorContent.Load<SpriteFont>("GizmoFont"));
            Reset();
            
            AfterInitializeEvent(); // set game content in logic, handle content & scene from parameter loading
            this.MainForm.Logic.History.UpdateEvent += delegate(HistoryManager manager)
            {
                Invalidate();
            };
            CameraHistory.UpdateEvent += delegate(CameraHistory history)
            {
                Invalidate();
            };

            _stopWatch = new Stopwatch();
            _stopWatch.Start();
            _timer = new Timer();
            _timer.Interval = 15;
            _timer.Tick += TimerOnTick;
            _timer.Start();
            ResetStopwatchDelta();
        }

        private void ResetStopwatchDelta()
        {
            _lastElapsed = _stopWatch.ElapsedMilliseconds;
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
            SceneWrapper scene = MainForm.Logic.WrappedScene;

            if (scene != null)
            {
                // wrong stencil fix:
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                //GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                scene.Draw(MainForm.CurrentDrawerStrategy, new GameTime(TimeSpan.Zero, TimeSpan.Zero));
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Grid.Draw();

            Gizmo.Update();
            Gizmo.Draw();

            _spriteBatch.Begin();
            Vector2 position = new Vector2(5, 5);
            Color textColor = Color.White;
            if (!_hasFocus)
            {
                Text = "[Focus lost]";
                textColor = Color.Orange;
            }
            Text = string.Format("Counter: {0}", _counter);
            _counter++;
            _spriteBatch.DrawString(_osdFont, Text, position, textColor, 0.0f, Vector2.Zero, new Vector2(0.75f), SpriteEffects.None, 0f);
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

        private void ViewportControl_MouseMove(object sender, MouseEventArgs e)
        {
            bool invalidate = true;
            _prevMouse = _currentMouse;
            _currentMouse = new EditorMouseState(_prevMouse) {X = e.X, Y = e.Y};
            _mouseMoved = true;
            switch (_currentMouse.OnlyButton)
            {
                case MouseBtn.Left:
                    if (Gizmo.MouseDrag(_currentMouse))
                    {
                        //MainForm.RefreshPropertyGrid();
                        //Update();
                        break;
                    }
                    break;

                case MouseBtn.Right:
                    Vector2 rot = (_prevMouse.Vector - _currentMouse.Vector) * _rotateSensitivity;
                    Camera.RotateYawPitch(rot.X, rot.Y);
                    Text = string.Format("Lookaround: {0}, {1}", rot.X, rot.Y);
                    UpdateCameraPosition();
                    Camera.Update();
                    break;

                case MouseBtn.None:
                    Gizmo.MouseHover(_currentMouse);
                    break;

                default:
                    invalidate = false;
                    break;
            }

            if (invalidate) Invalidate();
        }

        private void ViewportControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.Logic.WrappedScene == null) return;
            if (!_hasFocus)
            {
                this.Select();
            }
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _currentMouse.Left = true;
                    // select objects on scene / interact with gizmo
                    if (Gizmo.MouseDown(_currentMouse))
                    {
                        break;
                    }

                    // find intersected game object
                    Vector3 nearVector = new Vector3(_currentMouse.Vector, 0f);
                    Vector3 farVector  = new Vector3(_currentMouse.Vector, 1f);
                    Vector3 nearUnproj = GraphicsDevice.Viewport.Unproject(nearVector, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
                    Vector3 farUnproj  = GraphicsDevice.Viewport.Unproject(farVector,  Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
                    Vector3 direction  = farUnproj - nearUnproj;
                    direction.Normalize();
                    Ray picker = new Ray(nearUnproj, direction);

                    GameObjectWrapper collider = MainForm.Logic.WrappedScene.ClosestIntersector(picker);
                    if (collider != null)   // otherwise deselect, etc.
                    {
                        MainForm.SelectGameObject(collider);
                    }
                    break;

                case MouseButtons.Middle:
                    _currentMouse.Middle = true;
                    // camera strafe movement
                    break;

                case MouseButtons.Right:
                    _currentMouse.Right = true;
                    // FPS camera lookaround
                    Cursor.Current = Cursors.SizeAll;
                    //_oldDrawerStrategy = MainForm.CurrentDrawerStrategy;
                    //MainForm.CurrentDrawerStrategy = MainForm.BasicDrawerStrategy;

                    break;
            }
            _mouseMoved = false;
        }

        private void ViewportControl_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _currentMouse.Left = false;
                    Gizmo.MouseUp(_currentMouse);
                    break;
                case MouseButtons.Middle:
                    _currentMouse.Middle = false;
                    break;
                case MouseButtons.Right:
                    _currentMouse.Right = false;
                    Cursor.Current = Cursors.Default;
                    if(_mouseMoved) CameraHistory.NewPosition();
                    //MainForm.CurrentDrawerStrategy = _oldDrawerStrategy;
                    break;
            }
            _mouseMoved = false;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (_moveX == 0 && _moveY == 0) return;
            UpdateCameraPosition();
            Camera.Update();
            Invalidate();
        }

        private void UpdateCameraPosition()
        {
            long elapsed = _stopWatch.ElapsedMilliseconds;
            if (_currentMouse.OnlyButton == MouseBtn.Right)
            {
                long diff = elapsed - _lastElapsed;
                if (diff > 1000) diff = 1000;
                float dt = diff / 10.0f;

                if (_moveX != 0)
                {
                    Vector3 cameraStrafe = Vector3.Cross(Camera.Direction, Vector3.Up);
                    cameraStrafe.Normalize();
                    Camera.transform.Position += cameraStrafe * _moveX * _moveSensitivity * dt;
                }

                if (_moveY != 0)
                {
                    Camera.transform.Position += Camera.Direction * _moveY * _moveSensitivity * dt;
                }
            }
            _lastElapsed = elapsed;
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
            switch (_currentMouse.OnlyButton)
            {
                case MouseBtn.None:
                    // Handle camera history
                    if (e.KeyCode == Keys.Z && ModifierKeys == Keys.Shift && CameraHistory.CanUndo)
                    {
                        MainForm.UndoCameraMenuItem_Click(this, new EventArgs());
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Y && ModifierKeys == Keys.Shift && CameraHistory.CanRedo)
                    {
                        MainForm.RedoCameraMenuItem_Click(this, new EventArgs());
                        e.Handled = true;
                    }
                    else if (ModifierKeys == Keys.None)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                Gizmo.ActiveMode = GizmoMode.Translate;
                                break;
                            case Keys.E:
                                Gizmo.ActiveMode = GizmoMode.Rotate;
                                break;
                            case Keys.R:
                                Gizmo.ActiveMode = GizmoMode.UniformScale;
                                break;
                            case Keys.X:
                                Gizmo.ToggleActiveSpace();
                                break;
                        }
                        Invalidate();
                    }
                    break;

                case MouseBtn.Left:
                    if (e.KeyCode == Keys.ControlKey)
                    {
                        Gizmo.SnapEnabled = !MainForm.SnapEnabled;
                    }
                    break;

                case MouseBtn.Right:
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                        case Keys.W:
                            _moveY = 1;
                            ResetStopwatchDelta();
                            break;

                        case Keys.Down:
                        case Keys.S:
                            _moveY = -1;
                            ResetStopwatchDelta();
                            break;

                        case Keys.Left:
                        case Keys.A:
                            _moveX = -1;
                            ResetStopwatchDelta();
                            break;

                        case Keys.Right:
                        case Keys.D:
                            _moveX = 1;
                            ResetStopwatchDelta();
                            break;

                        case Keys.ShiftKey:
                            _moveSensitivity = BASE_MOVE_SENSITIVITY*10f;
                            break;

                        case Keys.ControlKey:
                            _moveSensitivity = BASE_MOVE_SENSITIVITY*0.1f;
                            _rotateSensitivity = BASE_ROTATE_SENSITIVITY*0.4f;
                            break;
                    }
                    break;
            }
        }


        private void ViewportControl_KeyUp(object sender, KeyEventArgs e)
        {
            //if (!_currentMouse.Right) return;

            if (e.KeyCode == Keys.ControlKey)
            {
                Gizmo.SnapEnabled = MainForm.SnapEnabled;
            }

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