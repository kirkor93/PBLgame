using System;
using System.Collections.Generic;
using Edytejshyn.Logic;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    /// <summary>
    /// Camera history manager for undoing & redoing camera position.
    /// </summary>
    public class CameraHistory
    {
        #region Variables

        private EditorLogger _logger;
        private Camera _camera;
        private readonly Stack<CameraState> _backwardStack = new Stack<CameraState>();
        private readonly Stack<CameraState> _forwardStack  = new Stack<CameraState>();
        private CameraState _currentState;

        /// <summary>
        /// Delegate for handling update events.
        /// </summary>
        public delegate void UpdateHandler(CameraHistory history);

        /// <summary>
        /// Event called every use of manager - undo, redo, etc..
        /// </summary>
        public event UpdateHandler UpdateEvent = (manager) => { };

        #endregion

        #region Properties
        /// <summary>
        /// Gets next undo Command or null if nothing to undo.
        /// </summary>
        public CameraState NextUndo 
        {
            get { return CanUndo ? _backwardStack.Peek() : null; }
        }

        /// <summary>
        /// Gets next redo Command or null if nothing to redo.
        /// </summary>
        public CameraState NextRedo
        {
            get { return CanRedo ? _forwardStack.Peek() : null;  }
        }

        public bool CanUndo { get { return _backwardStack.Count != 0; } }
        public bool CanRedo { get { return  _forwardStack.Count != 0; } }

        #endregion

        #region Methods

        public CameraHistory(EditorLogger logger, Camera camera)
        {
            _logger = logger;
            _camera = camera;
            SaveCurrentState();
        }

        private void SaveCurrentState()
        {
            _currentState = new CameraState(_camera);
        }

        /// <summary>
        /// Call this method to push new state of camera onto stack.
        /// </summary>
        public void NewPosition()
        {
            _forwardStack.Clear();
            _backwardStack.Push(_currentState);
            SaveCurrentState();
            UpdateEvent(this);
        }

        public void Undo()
        {
            if (_backwardStack.Count == 0) return;

            _forwardStack.Push(_currentState);
            _currentState = _backwardStack.Pop();
            _currentState.ApplyState(_camera);
            _logger.Log(LoggerLevel.Info, String.Format("Undo camera position"));
            UpdateEvent(this);
        }

        public void Redo()
        {
            if (_forwardStack.Count == 0) return;

            _backwardStack.Push(_currentState);
            _currentState = _forwardStack.Pop();
            _currentState.ApplyState(_camera);
            _logger.Log(LoggerLevel.Info, String.Format("Redo camera position"));
            UpdateEvent(this);
        }

        public void Clear()
        {
            _backwardStack.Clear();
            _forwardStack.Clear();
            UpdateEvent(this);
        }

        #endregion
    }
}
