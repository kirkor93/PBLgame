using System;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{

    public abstract class Light : GameObject
    {
        #region Variables
        private Vector4 _color;
        private LightType _type;
        #endregion

        #region Properites
        public Vector4 Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public LightType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        #endregion

        #region  Methods
        public Light()
        {
        }

        public void Upate()
        {
            base.Update();
        }

        public void Draw()
        {
            base.Draw();
        }
        #endregion
    }

#region LightEnumType
    public enum LightType
    {
        directional = 0,
        point
    }
#endregion
}
