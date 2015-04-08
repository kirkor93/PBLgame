using System;

using PBLgame.Engine.Components;

using Microsoft.Xna.Framework;

namespace PBLgame.Engine.GameObjects
{
    public class MyDirectionalLight : Light
    {
        #region Variables
        private float _intensity;
        #endregion

        #region Properites
        public float Intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                _intensity = value;
            }
        }
        

        public Vector3 Direction
        {
            get
            {
                return transform.Position;
            }
            set
            {
                transform.Position = value;
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
