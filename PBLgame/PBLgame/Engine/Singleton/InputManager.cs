using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PBLgame.Engine.Singleton
{
    #region Event Helpers
    public delegate void EventHandler(Object sender, EventArgs e);

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
            public event EventHandler OnMove;
            public event EventHandler OnTurn;
            public event EventHandler OnButton;
            #endregion

            #region Private
            private GamePadState _gamePadState;
            //private GamePadButtons _buttons;
            //private GamePadThumbSticks _sticks;
            //private GamePadTriggers _triggers;
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
            //_buttons = new GamePadButtons();
            //_sticks = new GamePadThumbSticks();
            //_triggers = new GamePadTriggers();
            OnMove += Debug;
        }

        public void Update()
        {
            if(_gamePadState.IsConnected)
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
                if (_gamePadState.ThumbSticks.Right.Length() != 0.0f)
                {
                    if (OnMove != null)
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

        void Debug(Object obj, EventArgs e)
        {
            e.ToString();
        }
    #endregion
    }
}
