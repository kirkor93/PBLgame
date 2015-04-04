namespace Edytejshyn.Logic
{
    /// <summary>
    /// Command pattern command for history manager (undo/redo)
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Tells manager whether the command changes data which should be saved.
        /// </summary>
        bool AffectsData { get; }

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
