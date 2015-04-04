using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.Logic.Commands
{
    public class CameraCommand : ICommand
    {

        private Vector3[] _direction = new Vector3[2];
        private Transform[] _transform = new Transform[2];
        private Camera _camera;

        public CameraCommand(Camera camera)
        {
            _camera = camera;
            SaveState(0);
        }

        public void SaveFinalState()
        {
            SaveState(1);
        }

        public bool AffectsData
        {
            get { return false; }
        }

        public string Description
        {
            get { return "Camera move"; }
        }

        public string Message
        {
            get { return "Camera position changed"; }
        }

        public void Do()
        {
            ApplyState(1);
        }

        public void Undo()
        {
            ApplyState(0);
        }

        private void SaveState(int i)
        {
            _direction[i] = _camera.Direction;
            _transform[i] = new Transform(_camera.transform);
        }

        private void ApplyState(int i)
        {
            _camera.Direction = _direction[i];
            _camera.transform = new Transform(_transform[i]);
        }

    }
}