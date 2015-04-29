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
                return "X=" + AxisValue.X + " ,Y=" + AxisValue.Y;
            }
    }
    
    public class  ButtonArgs : EventArgs
    {
        public Buttons ButtonName
        {
            get;
            set;
        }
        public ButtonState ButtState
        {
            get;
            set;
        }

        public ButtonArgs(Buttons name,ButtonState state)
        {
            ButtonName = name;
            ButtState = state;
        }
    }
    #endregion

    public class InputManager : Singleton<InputManager>
    {
        #region Variables
            #region Public
            public event StickHandler OnMove;
            public event StickHandler OnTurn;
            public event ButtonHandler OnButton;
            #endregion

            #region Private
            private GamePadState _gamePadState;
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
            if (_gamePadState.IsConnected)
            {
                //Left stick
                if (_gamePadState.ThumbSticks.Left.Length() != 0.0f)
                {
                    if(OnMove != null)
                    {
                        OnMove(this, new MoveArgs(_gamePadState.ThumbSticks.Left));
                    }
                }
                
                //Right stick
                if (_gamePadState.ThumbSticks.Right.Length() > 0.3f)
                {
                    if (OnTurn != null)
                    {
                        OnTurn(this, new MoveArgs(_gamePadState.ThumbSticks.Right));
                    }
                }

                //Buttons, thats looking sad ;c
                if(_gamePadState.IsButtonDown(Buttons.A))
                {
                    if(OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.A, ButtonState.Pressed));
                    }
                }
                if(_gamePadState.IsButtonDown(Buttons.X))
                {
                    if (OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.X, ButtonState.Pressed));
                    }
                }
                if (_gamePadState.IsButtonDown(Buttons.LeftShoulder))
                {
                    if (OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.LeftShoulder, ButtonState.Pressed));
                    }
                }
                if (_gamePadState.IsButtonDown(Buttons.RightShoulder))
                {
                    if (OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.RightShoulder, ButtonState.Pressed));
                    }
                }
                if (_gamePadState.IsButtonDown(Buttons.LeftTrigger))
                {
                    if (OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.LeftTrigger, ButtonState.Pressed));
                    }
                }
                if (_gamePadState.IsButtonDown(Buttons.RightTrigger))
                {
                    if (OnButton != null)
                    {
                        OnButton(this, new ButtonArgs(Buttons.RightTrigger, ButtonState.Pressed));
                    }
                }
            }
        }
    #endregion
    }
}
