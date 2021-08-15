namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Indicates an object which has a parent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentable<T>
    {
        /// <summary>
        /// The parent.
        /// </summary>
        T Parent { get; }
    }
}
