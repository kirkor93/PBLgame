using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Transform : Component
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        private Matrix _worldTranslation;
        private Matrix _worldRotation;
        private Matrix _worldScale;
        private Matrix _world;
        #endregion
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _worldTranslation = Matrix.CreateTranslation(value);
                _position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                RotationLimit();
                _worldRotation = Matrix.CreateFromYawPitchRoll(value.X, value.Y, value.Z);
            }
        }
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _worldScale = Matrix.CreateScale(value);
                _scale = value;
            }
        }
        public Matrix World
        {
            get
            {
                return _world = _worldScale * _worldRotation * _worldTranslation;
            }
        }
        #endregion

        #region Methods

        public Transform(GameObject owner) : base(owner)
        {
            _position = Vector3.Zero;
            _rotation = Vector3.Zero;
            _scale = new Vector3(1.0f, 1.0f, 1.0f);

            _worldTranslation = Matrix.Identity;
            _worldRotation = Matrix.Identity;
            _worldScale = Matrix.Identity;
        }

        public void Translate(Vector3 trans)
        {
            _position += trans;
            _worldTranslation = Matrix.CreateTranslation(_position);
            _world = _worldTranslation;            
        }
        public void Translate(float x,float y, float z)
        {
            _position += new Vector3(x,y,z);
            _worldTranslation = Matrix.CreateTranslation(_position);
            _world = _worldTranslation;
        }
        public void Rotate(Vector3 rot)
        {
            _rotation += rot;
            RotationLimit();
            _worldRotation = Matrix.CreateFromYawPitchRoll(_rotation.X,_rotation.Y,_rotation.Z);
            _world = _worldRotation;
        }
        public void Rotate(float x, float y, float z)
        {
            _rotation += new Vector3(x, y, z);
            RotationLimit();
            _worldRotation = Matrix.CreateFromYawPitchRoll(x, y, z);
            _world = _worldRotation;
        }
        #endregion

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }

        private void RotationLimit()
        {
            _rotation.X = _rotation.X % 360;
            _rotation.Y = _rotation.Y % 360;
            _rotation.Z = _rotation.Z % 360;
        }
    }
}
