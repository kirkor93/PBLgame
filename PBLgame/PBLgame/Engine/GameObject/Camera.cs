using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Engine.GameObject
{
    class Camera : GameObject
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Matrix view;
        private Matrix projection;
        private float near;
        private float far;
        private float foV;
        #endregion
        #endregion

        #region Properites
        public Matrix View
        {
            get
            {
                return this.view;
            }
        }
        public Matrix Projection
        {
            get
            {
                return this.projection;
            }
        }
        #endregion

        #region Methods
        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up,
            float FoV, float screenWidth, float screenHeight,float near, float far)
        {
            this.foV = FoV;
            this.near = near;
            this.far = far;
            view = Matrix.CreateLookAt(pos, target, up);

            projection = Matrix.CreatePerspectiveFieldOfView(
                foV,
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
