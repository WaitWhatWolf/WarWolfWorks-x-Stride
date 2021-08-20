using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on an <see cref="INyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a FixedUpdate method.
    /// </summary>
    public interface INyuFixedUpdate
    {
        /// <summary>
        /// Called every physics frame.
        /// </summary>
        void NyuFixedUpdate();
    }
}
