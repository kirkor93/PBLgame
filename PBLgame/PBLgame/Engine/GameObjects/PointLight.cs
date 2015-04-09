using System;

using Microsoft.Xna.Framework;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{
    public class PointLight : Light
    {
        #region Variables
        private float _attenuation;
        private float _fallOff;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return transform.Position;
            }
            set
            {
                if(parent != null)
                {
                    transform.Position = parent.transform.Position + value;
                }
                else
                {
                    transform.Position = value;
                }
            }
        }

        public float Attenuation
        {
            get
            {
                return _attenuation;
            }
            set
            {
                _attenuation = value;
            }
        }

        public float FallOff
        {
            get
            {
                return _fallOff;
            }
            set
            {
                _fallOff = value;
            }
        }
        #endregion 

        #region Methods
        public void Update()
        {

        }

        public void Draw()
        {

        }
        #endregion
    }
}
