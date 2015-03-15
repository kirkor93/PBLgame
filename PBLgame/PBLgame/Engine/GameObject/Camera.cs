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
        #endregion
        #endregion

        #region Properites
        public Matrix ViewMatrix
        {
            get
            {
                return this._viewMatrix;
            }
        }
        public Matrix ProjectionMatrix
        {
            get
            {
                return this._projectionMatrix;
            }
        }
        #endregion

        #region Methods
        public Camera(Microsoft.Xna.Framework.Game game, Vector3 pos, Vector3 target, Vector3 up,
            float FoV, float screenWidth, float screenHeight,float near, float far)
        {
            this._foV = FoV;
            this._near = near;
            this._far = far;
            _viewMatrix = Matrix.CreateLookAt(pos, target, up);

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

        }
        #endregion
    }
}
