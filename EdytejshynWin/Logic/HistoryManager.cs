using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edytejshyn.Logic
{
    /// <summary>
    /// History manager with command pattern. For undoing & redoing.
    /// </summary>
    public class HistoryManager
    {
        #region Variables

        private EditorLogic _logic;
        private Stack<ICommand> _backwardStack = new Stack<ICommand>();
        private Stack<ICommand> _forwardStack = new Stack<ICommand>();

        /// <summary>
        /// Delegate for handling update events.
        /// </summary>
        public delegate void UpdateHandler(HistoryManager manager);

        /// <summary>
        /// Event called every use of manager - undo, redo, etc..
        /// </summary>s
        public event UpdateHandler UpdateEvent = (manager) => { };

        #endregion

        #region Properties
        /// <summary>
        /// Gets next undo Command or null if nothing to undo.
        /// </summary>
        public ICommand NextUndo 
        {
            get { return _backwardStack.Count == 0  ? null : _backwardStack.Peek(); }
        }

        /// <summary>
        /// Gets next redo Command or null if nothing to redo.
        /// </summary>
        public ICommand NextRedo
        {
            get { return _forwardStack.Count == 0  ? null : _forwardStack.Peek();  }
        }
        #endregion

        #region Methods

        public HistoryManager(EditorLogic logic)
        {
            _logic = logic;
        }

        /// <summary>
        /// Call this method every new action made in application. Command will be fired and gone down in history.
        /// </summary>
        /// <param name="command">Command to be fired and added to command history.</param>
        public void NewAction(ICommand command)
        {
            _forwardStack.Clear();
            command.Do();
            _backwardStack.Push(command);
            _logic.Logger.Log(LoggerLevel.Info, command.Message);
            UpdateEvent(this);
        }

        public void Undo()
        {
            if (_backwardStack.Count != 0)
            {
                ICommand cmd = _backwardStack.Pop();
                cmd.Undo();
                _forwardStack.Push(cmd);
                _logic.Logger.Log(LoggerLevel.Info, String.Format("Undo: {0}", cmd.Message));
            }
            UpdateEvent(this);
        }

        public void Redo()
        {
            if (_forwardStack.Count != 0)
            {
                ICommand cmd = _forwardStack.Pop();
                cmd.Do();
                _backwardStack.Push(cmd);
                _logic.Logger.Log(LoggerLevel.Info, String.Format("Redo: {0}", cmd.Message));
            }
            UpdateEvent(this);
        }

        #endregion
    }
}
