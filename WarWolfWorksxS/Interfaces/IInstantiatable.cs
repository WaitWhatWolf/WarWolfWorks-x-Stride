namespace WarWolfWorksxS.Interfaces
{
    /// <summary>
    /// Indicates an instantiatable object.
    /// </summary>
    public interface IInstantiatable<T>
    {
#pragma warning disable 0649
        /// <summary>
        /// Gets the copy of this <see cref="IInstantiatable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public T GetCopy();
        /// <summary>
        /// Instantiate sub-objects.
        /// </summary>
        void PostInstantiate();
    }
}
