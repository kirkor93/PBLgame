using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    /// <summary>
    /// Represents camera transformation & direction for further undoing.
    /// </summary>
    public class CameraState
    {
        private readonly Vector3 _direction;
        private readonly Transform _transform;

        public CameraState(Camera camera)
        {
            _direction = camera.Direction;
            _transform = new Transform(camera.transform);
        }

        public void ApplyState(Camera camera)
        {
            camera.Direction = _direction;
            camera.transform = new Transform(_transform);
        }


    }
}