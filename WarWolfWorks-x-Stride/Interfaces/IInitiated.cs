namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Indicates an object which has an initiated state.
    /// </summary>
    public interface IInitiated
    {
        /// <summary>
        /// Returns the initiated state of this <see cref="IInitiated"/>.
        /// </summary>
        bool Initiated { get; }
    }
}
