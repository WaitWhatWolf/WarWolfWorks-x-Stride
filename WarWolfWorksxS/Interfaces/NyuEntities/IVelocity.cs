using Stride.Core.Mathematics;
using WarWolfWorksxS.NyuEntities;
using WarWolfWorksxS.NyuEntities.MovementSystem;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// A velocity component to be used with <see cref="NyuMovement"/>.
    /// Supported interfaces: <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>.
    /// </summary>
    public interface IVelocity : IParentInitiatable<NyuMovement>, INyuReferencable
    {
        /// <summary>
        /// The final value applied to this velocity.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetValue();
    }
}
