using Stride.Engine;
using WarWolfWorks_x_Stride.NyuEntities;

namespace WarWolfWorks_x_Stride.Interfaces.NyuEntities
{
    /// <summary>
    /// Indicates an object that uses a reference to an <see cref="Entity"/>. (usually owner)
    /// </summary>
    public interface INyuReferencable
    {
        /// <summary>
        /// The <see cref="Entity"/> reference of this <see cref="INyuReferencable"/>.
        /// </summary>
        public Nyu NyuMain { get; }
    }
}
