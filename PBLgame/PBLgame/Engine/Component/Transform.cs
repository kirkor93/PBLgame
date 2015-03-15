using Microsoft.Xna.Framework;

namespace PBLgame.Engine.Component
{
    public class Transform : IComponent
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        private Matrix _worldTranslate;
        private Matrix _worldRotation;
        private Matrix _worldScale;
        #endregion
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return this._rotation;
            }
            set
            {
                this._rotation = value;
            }
        }
        public Vector3 Scale
        {
            get
            {
                return this._scale;
            }
            set
            {
                this._scale = value;
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
