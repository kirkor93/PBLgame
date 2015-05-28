using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine.GUI
{
    public class Bar : GUIObject
    {
        private float _fillAmount;

        public float FillAmount
        {
            get { return _fillAmount; }
            set
            {
                _fillAmount = value <= 1.0f ? value : 1.0f;
                if (_fillAmount < 0.0f)
                {
                    _fillAmount = 0.0f;
                }
                UpdateBoundries();
            }
        }

        protected override void UpdateBoundries()
        {
            base.UpdateBoundries();
            _boundries.Width = (int)(_boundries.Width * FillAmount);
        }
    }
}