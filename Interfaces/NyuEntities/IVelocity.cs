using Stride.Core.Mathematics;
using WarWolfWorks_x_Stride.NyuEntities;
using WarWolfWorks_x_Stride.NyuEntities.MovementSystem;

namespace WarWolfWorks_x_Stride.Interfaces.NyuEntities
{
    /// <summary>
    /// A velocity component to be used with <see cref="NyuMovement"/>.
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
