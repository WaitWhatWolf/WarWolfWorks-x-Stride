using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="INyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a Start method.
    /// </summary>
    public interface INyuStart
    {
        /// <summary>
        /// Invoked at creation, after <see cref="INyuAwake"/>.
        /// </summary>
        void NyuStart();
    }
}
