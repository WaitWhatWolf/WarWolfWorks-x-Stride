using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="AsyncNyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a Update method.
    /// </summary>
    public interface INyuUpdate
    {
        /// <summary>
        /// Invoked every in-game frame.
        /// </summary>
        void NyuUpdate();
    }
}
