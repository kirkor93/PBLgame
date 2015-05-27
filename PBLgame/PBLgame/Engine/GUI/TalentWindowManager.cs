using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class TalentWindowManager
    {
        private Button _selectedButton;
        private List<Button> _guiButtons;
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (Background != null)
                {
                    Background.Enabled = value;
                }
                foreach (Button button in _guiButtons)
                {
                    button.Enabled = value;
                }
            }
            
        }

        public GUIObject Background { get; set; }

        public List<Button> GuiButtons
        {
            get { return _guiButtons; }
            set
            {
                _guiButtons = value;
                if (_guiButtons.Count > 0)
                {
                    _selectedButton = _guiButtons.FirstOrDefault();
                    ChangeHighlightedButton(_selectedButton);
                }
            }
        }

        public TalentWindowManager()
        {
            _guiButtons = new List<Button>();
            InputManager.Instance.OnButton += OnButtonClick;
        }

        private void OnButtonClick(object sender, ButtonArgs e)
        {
            if (e.ButtonName == Buttons.Y)
            {
                Enabled = true;
                Background.Enabled = true;
                foreach (Button button in _guiButtons)
                {
                    button.Enabled = true;
                }
            }

            if (Enabled && e.IsDown)
            {
                switch (e.ButtonName)
                {
                    case Buttons.DPadDown:
                        ChangeHighlightedButton(_selectedButton.DownNeighbour);
                        break;
                    case Buttons.DPadLeft:
                        ChangeHighlightedButton(_selectedButton.LeftNeighbour);
                        break;
                    case Buttons.DPadRight:
                        ChangeHighlightedButton(_selectedButton.RightNeighbour);
                        break;
                    case Buttons.DPadUp:
                        ChangeHighlightedButton(_selectedButton.UpNeighbour);
                        break;
                    case Buttons.A:
                        if (_selectedButton != null)
                        {
                            _selectedButton.OnClick();
                        }
                        break;
                    case Buttons.B:
                        Enabled = false;

                        break;
                }
            }
        }

        private void ChangeHighlightedButton(Button target)
        {
            if (target != null)
            {
                _selectedButton.Texture = _selectedButton.EnabledTexture;
                target.Texture = target.SelectedTexture;
                _selectedButton = target;
            }
        }
    }
}