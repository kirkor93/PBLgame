using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Singleton;
using PBLgame.GamePlay;

namespace PBLgame.Engine.GUI
{
    public class TalentWindowManager
    {
        private Button _selectedButton;
        private List<Button> _guiButtons;
        private bool _enabled;
        private PlayerStatistics _playerStatistics;
        private string _originalAvilablePointsText;
        private GUIObject _background;

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

        public GUIObject Background
        {
            get { return _background; }
            set
            {
                _background = value;
                if (value.Text != null)
                {
                    _originalAvilablePointsText = string.Copy(value.Text.Text);
                }
            }
            
        }

        public List<Button> GuiButtons
        {
            get { return _guiButtons; }
            set
            {
                _guiButtons = value;
                if (_guiButtons.Count > 0)
                {
                    _selectedButton = _guiButtons.FirstOrDefault();
                    ChangeHighlightedButton(_selectedButton, Buttons.DPadDown);
                }
            }
        }

        public TalentWindowManager()
        {
            _guiButtons = new List<Button>();
            InputManager.Instance.OnButton += OnButtonClick;
        }

        public void AssignPlayerStatisticsScript(PlayerStatistics stats)
        {
            _playerStatistics = stats;
        }

        private void OnButtonClick(object sender, ButtonArgs e)
        {
            if (e.ButtonName == Buttons.Y)
            {
                RefreshAvilablePointsCounter();
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
                        ChangeHighlightedButton(_selectedButton.DownNeighbour, e.ButtonName);
                        break;
                    case Buttons.DPadLeft:
                        ChangeHighlightedButton(_selectedButton.LeftNeighbour, e.ButtonName);
                        break;
                    case Buttons.DPadRight:
                        ChangeHighlightedButton(_selectedButton.RightNeighbour, e.ButtonName);
                        break;
                    case Buttons.DPadUp:
                        ChangeHighlightedButton(_selectedButton.UpNeighbour, e.ButtonName);
                        break;
                    case Buttons.A:
                        if (_selectedButton != null)
                        {
                            _selectedButton.Texture = _selectedButton.PressedTexture;
                        }
                        break;
                    case Buttons.B:
                        Enabled = false;
                        break;
                }
            }
            else if(Enabled && e.ButtonName == Buttons.A)
            {
                _selectedButton.Texture = _selectedButton.SelectedTexture;
                _selectedButton.OnClick(_selectedButton, _playerStatistics);
                RefreshAvilablePointsCounter();
            }
        }

        private void ChangeHighlightedButton(Button target, Buttons button)
        {
            while (target != null)
            {
                if (target.Texture != target.DisabledTexture)
                {
                    _selectedButton.Texture = _selectedButton.EnabledTexture;
                    target.Texture = target.SelectedTexture;
                    _selectedButton = target;
                    return;
                }
                switch (button)
                {
                    case Buttons.DPadDown:
                        target = target.DownNeighbour;
                        break;
                    case Buttons.DPadUp:
                        target = target.UpNeighbour;
                        break;
                    case Buttons.DPadLeft:
                        target = target.LeftNeighbour;
                        break;
                    case Buttons.DPadRight:
                        target = target.RightNeighbour;
                        break;
                }
            }
        }

        private void RefreshAvilablePointsCounter()
        {
            if (Background != null && Background.Text != null)
            {
                Background.Text.Text = string.Format("{0}{1}", _originalAvilablePointsText,
                    _playerStatistics.TalentPoints.Value.ToString("G", CultureInfo.InvariantCulture));

            }
        }
    }
}