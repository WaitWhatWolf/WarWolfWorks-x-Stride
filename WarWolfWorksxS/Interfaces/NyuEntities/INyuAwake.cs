using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="INyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has an Awake method.
    /// </summary>
    public interface INyuAwake
    {
        /// <summary>
        /// Invoked at creation, before <see cref="INyuStart"/>.
        /// </summary>
        void NyuAwake();
    }
}
