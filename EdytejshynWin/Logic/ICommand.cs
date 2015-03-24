namespace Edytejshyn.Logic
{
    /// <summary>
    /// Command pattern command for history manager (undo/redo)
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Short description of the command action. Could be visible in Edit -> Undo: The Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Message for logger what has been done.
        /// </summary>
        string Message { get; }

        void Do();
        void Undo();
    }
}
