using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PBLgame.Engine.Singleton
{
    #region Event Helpers
    public delegate void StickHandler(Object sender, MoveArgs e);
    public delegate void ButtonHandler(Object sender, ButtonArgs e);

    public class MoveArgs : EventArgs
    {
            public Vector2 AxisValue
            {
                get;
                set;
            }

            public MoveArgs(Vector2 stick)
            {
                AxisValue = stick;
            }

            public override string ToString()
            {
                return string.Format("X={0}, Y={1}", AxisValue.X, AxisValue.Y);
            }
    }
    
    public class  ButtonArgs : EventArgs
    {
        public Buttons ButtonName { get; set; }
        public bool IsDown { get; set; }

        public ButtonArgs(Buttons name, bool isDown)
        {
            ButtonName = name;
            IsDown = isDown;
        }
    }
    #endregion

    /// <summary>
    /// Provides input control events & hints for GUI.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        #region Variables
            #region Public
            public event StickHandler OnMove;
            public event StickHandler OnTurn;
            public event ButtonHandler OnButton;
            #endregion

            #region Private

            private static readonly Buttons[] ButtonsArray = { Buttons.A, Buttons.B, Buttons.X, Buttons.Y, Buttons.LeftShoulder, Buttons.RightShoulder, Buttons.LeftTrigger, Buttons.RightTrigger, Buttons.DPadDown, Buttons.DPadUp, Buttons.DPadLeft, Buttons.DPadRight };
            private bool[] _buttonsDown = new bool[ButtonsArray.Length];
            private GamePadState _gamePadState;
            private int _lastPacketNumber = 0;
            private bool _rumble;
            private double _rumbleMiliseconds;
            private Vector2 _lastLeftStick, _lastRightStick;

        #endregion
        #endregion

        #region Methods
        public InputManager()
            {
                OnMove = null;
                OnTurn = null;
                OnButton = null;
            }

        public void Initialize()
        {
            _gamePadState = new GamePadState();
        }

        public void Update(GameTime gameTime)
        {
            UpdateRumble(gameTime);

            _gamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            int packetNumber = _gamePadState.PacketNumber;
            
            if (!_gamePadState.IsConnected || packetNumber == _lastPacketNumber) return;

            _lastPacketNumber = packetNumber;

            //Left stick
            if (_gamePadState.ThumbSticks.Left != _lastLeftStick)
            {
                _lastLeftStick = _gamePadState.ThumbSticks.Left;

                if(OnMove != null)
                {
                    OnMove(this, new MoveArgs(Circularize(_gamePadState.ThumbSticks.Left)));
                }
            }
                
            //Right stick
            if (_gamePadState.ThumbSticks.Right != _lastRightStick)
            {
                _lastRightStick = _gamePadState.ThumbSticks.Right;
                if (OnTurn != null)
                {
                    OnTurn(this, new MoveArgs(Circularize(_gamePadState.ThumbSticks.Right)));
                }
            }


            for (int i = 0; i < ButtonsArray.Length; i++)
            {
                Buttons button = ButtonsArray[i];
                bool down = _gamePadState.IsButtonDown(button);
                if (down == _buttonsDown[i]) continue;
                
                if (OnButton != null)
                {
                    OnButton(this, new ButtonArgs(button, down));
                }
                _buttonsDown[i] = down;
            }

        }

        private Vector2 Circularize(Vector2 input)
        {
            float length = input.Length();
            if (length == 0f) return Vector2.Zero;
            float mul =  MathHelper.Clamp(length, -1f, 1f) / length;
            return input * mul;
        }

        /// <summary>
        /// Turns on rumbling (vibrating) force feedback on gamepad.
        /// Power of motors in range [0.0 .. 1.0].
        /// </summary>
        /// <param name="miliseconds">time of rumbling in ms</param>
        /// <param name="lowMotor">left low frequency motor power</param>
        /// <param name="highMotor">right high frequency motor power</param>
        public void RumplePad(double miliseconds, float lowMotor, float highMotor)
        {
            bool success = GamePad.SetVibration(PlayerIndex.One, lowMotor, highMotor);
            _rumble = true;
            _rumbleMiliseconds = miliseconds;
        }

        private void UpdateRumble(GameTime gameTime)
        {
            if (!_rumble) return;

            _rumbleMiliseconds -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_rumbleMiliseconds > 0) return;

            _rumbleMiliseconds = 0;
            bool success = GamePad.SetVibration(PlayerIndex.One, 0, 0);
            if (success) _rumble = false;
        }

        #endregion
    }
}
