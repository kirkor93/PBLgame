using System.Dynamic;
using Microsoft.Xna.Framework;

namespace PBLgame.Engine.GameObjects
{
    public class Camera : GameObject
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
        private Vector3 _direction;

        //static reference to first camera created for other classes
        private static Camera _mainCamera = null;
        #endregion
        #endregion

        #region Properites
        public Matrix ViewMatrix
        {
            get
            {
                _viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction + base.transform.Position, Vector3.Up);
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

        public static Camera MainCamera 
        {
            get
            {
                return _mainCamera;
            }
            private set
            {
                _mainCamera = value;
            }
        }
    
        #endregion

        #region Methods
        public Camera(Vector3 pos, Vector3 target, Vector3 up,
            float FoV, float screenWidth, float screenHeight,float near, float far)
        {
            base.transform.Position = pos;
            _direction = target - pos;
            _direction.Normalize();

            _foV = FoV;
            _near = near;
            _far = far;
            _viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction+pos, up);

            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                _foV,
                screenWidth / screenHeight,
                near, far);


            //first camera created is main camera for game
            if (MainCamera == null)
            {
                MainCamera = this;
            }

        }

        public void Initialize()
        {

        }

        public void Update(GameTime gameTime)
        {
            _viewMatrix = Matrix.CreateLookAt(base.transform.Position, _direction + base.transform.Position, Vector3.Up);
        }

        #endregion

        // FOR TESTING INPUT EVENTS================
        public void EventMove(object obj, Singleton.MoveArgs args)
        {
            args.AxisValue = args.AxisValue * 0.01f;
            base.transform.Translate(args.AxisValue.X, args.AxisValue.Y, 0.0f);
        }
        //=======================================
    }
}
