namespace WarWolfWorksxS.Interfaces
{
    /// <summary>
    /// Used for basic initiation of an object.
    /// </summary>
    public interface IInitiatable : IInitiated
    {
        /// <summary>
        /// Initiates this <see cref="IInitiatable"/>.
        /// </summary>
        bool Init();
    }
}
