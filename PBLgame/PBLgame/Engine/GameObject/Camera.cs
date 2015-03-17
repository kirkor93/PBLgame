using Microsoft.Xna.Framework;

namespace PBLgame.Engine.GameObject
{
    class Camera : GameObject
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private float _near;
        private float _far;
        private float _foV;
        private Components.Transform _transform;
        private Vector3 _direction;
        #endregion
        #endregion

        #region Properites
        public Matrix ViewMatrix
        {
            get
            {
                _viewMatrix = Matrix.CreateLookAt(_transform.Position, _direction+_transform.Position, Vector3.Up);
                return _viewMatrix;
            }
        }
        public Matrix ProjectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }
        public Components.Transform Transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
            }
        }
        public Vector3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }
        #endregion

        #region Methods
        public Camera(Vector3 pos, Vector3 target, Vector3 up,
            float FoV, float screenWidth, float screenHeight,float near, float far)
        {
            _transform = new Components.Transform();
            _transform.Position = pos;
            _direction = target - pos;
            _direction.Normalize();

            _foV = FoV;
            _near = near;
            _far = far;
            _viewMatrix = Matrix.CreateLookAt(_transform.Position, _direction+pos, up);

            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                _foV,
                screenWidth / screenHeight,
                near, far);
        }

        public void Initialize()
        {

        }

        public void Update(GameTime gameTime)
        {
            _viewMatrix = Matrix.CreateLookAt(_transform.Position, _direction + _transform.Position, Vector3.Up);
        }
        #endregion
    }
}
