using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Engine.Component
{
    class Transform : IComponent
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;

        private Matrix worldTranslate;
        private Matrix worldRotation;
        private Matrix worldScale;
        #endregion
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
            }
        }
        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }
        #endregion

        #region Methods
        public void Translate()
        {

        }
        public void Rotate()
        {

        }
        #endregion
    }
}
