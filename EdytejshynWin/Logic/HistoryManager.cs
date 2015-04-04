using System;
using System.Collections.Generic;

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
        /// Contains actions ahead of state saved in currently opened file.
        /// Increases every new action done after save. Decreases when undoing.
        /// Value == 0 means editor state is identical as in file (like after opening).
        /// Value &gt; 0 indicates how many actions have been done after saving.
        /// Value &lt; 0 is when history has been reverted (undone) beyond saved point.
        /// </summary>
        public int AheadSaved { get; private set; }

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
            ChangeAheadSaved(command, 1);
            UpdateEvent(this);
        }

        /// <summary>
        /// Changes AheadSaved property only if command affects data.
        /// </summary>
        /// <param name="command">the command</param>
        /// <param name="change">change in counter to add</param>
        private void ChangeAheadSaved(ICommand command, int change)
        {
            if (command.AffectsData) AheadSaved += change;
        }

        public void Undo()
        {
            if (_backwardStack.Count == 0) return;

            ICommand cmd = _backwardStack.Pop();
            cmd.Undo();
            _forwardStack.Push(cmd);
            _logic.Logger.Log(LoggerLevel.Info, String.Format("Undo: {0}", cmd.Message));
            ChangeAheadSaved(cmd, -1);
            UpdateEvent(this);
        }

        public void Redo()
        {
            if (_forwardStack.Count == 0) return;

            ICommand cmd = _forwardStack.Pop();
            cmd.Do();
            _backwardStack.Push(cmd);
            _logic.Logger.Log(LoggerLevel.Info, String.Format("Redo: {0}", cmd.Message));
            ChangeAheadSaved(cmd, 1);
            UpdateEvent(this);
        }

        public void Clear()
        {
            AheadSaved = 0;
            _backwardStack.Clear();
            _forwardStack.Clear();
            UpdateEvent(this);
        }

        public void SetSavedPoint()
        {
            AheadSaved = 0;
            UpdateEvent(this);
        }

        #endregion
    }
}
