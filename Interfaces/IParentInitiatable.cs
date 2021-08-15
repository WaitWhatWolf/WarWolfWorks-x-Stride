namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Used for basic initiation of a child object.
    /// </summary>
    public interface IParentInitiatable<T> : IInitiated, IParentable<T>
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        bool Init(T parent);
    }
}
