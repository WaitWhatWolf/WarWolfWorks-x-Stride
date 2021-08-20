using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="AsyncNyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has fuctionality when destroyed.
    /// </summary>
    public interface INyuOnDestroy
    {
        /// <summary>
        /// Invoked right before this INyuOnDestroy is about to be removed.
        /// </summary>
        void NyuOnDestroy();
    }
}
