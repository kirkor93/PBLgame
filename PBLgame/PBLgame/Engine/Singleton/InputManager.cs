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

            private static readonly Buttons[] ButtonsArray = { Buttons.A, Buttons.B, Buttons.X, Buttons.Y, Buttons.LeftShoulder, Buttons.RightShoulder, Buttons.LeftTrigger, Buttons.RightTrigger };
            private bool[] _buttonsDown = new bool[ButtonsArray.Length];
            private GamePadState _gamePadState;
            private int _lastPacketNumber = 0;
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

        public void Update()
        {
            _gamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            int packetNumber = _gamePadState.PacketNumber;

            if (!_gamePadState.IsConnected || packetNumber == _lastPacketNumber) return;

            _lastPacketNumber = packetNumber;

            //Left stick
            if (_gamePadState.ThumbSticks.Left.Length() != 0.0f)
            {
                if(OnMove != null)
                {
                    OnMove(this, new MoveArgs(_gamePadState.ThumbSticks.Left));
                }
            }
                
            //Right stick
            if (_gamePadState.ThumbSticks.Right.LengthSquared() > 0.1f)
            {
                if (OnTurn != null)
                {
                    OnTurn(this, new MoveArgs(_gamePadState.ThumbSticks.Right));
                }
            }


            for (int i = 0; i < ButtonsArray.Length; i++)
            {
                Buttons button = ButtonsArray[i];
                bool down = _gamePadState.IsButtonDown(button);
                if (down == _buttonsDown[i]) continue;
                Console.WriteLine(string.Format("Down: {0}, i: {1}, packet: {2}", down, i, packetNumber));
                
                if (OnButton != null)
                {
                    OnButton(this, new ButtonArgs(button, down));
                }
                _buttonsDown[i] = down;
            }

        }
    #endregion
    }
}
