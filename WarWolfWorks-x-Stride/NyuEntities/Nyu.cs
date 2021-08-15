using Stride.Core.Mathematics;
using Stride.Engine;
using WarWolfWorks_x_Stride.NyuEntities.Statistics;

namespace WarWolfWorks_x_Stride.NyuEntities
{
    public abstract class Nyu : AsyncScript
    {
        /// <summary>
        /// The entity's stats manager.
        /// </summary>
        public Stats Stats { get; internal set; }

        /// <summary>
        /// The position of the parent.
        /// </summary>
        public virtual Vector3 Position { get => Entity.Transform.Position; set => Entity.Transform.Position = value; }

        /// <summary>
        /// The rotation of the parent.
        /// </summary>
        public virtual Quaternion Rotation { get => Entity.Transform.Rotation; set => Entity.Transform.Rotation = value; }

        /// <summary>
        /// The euler rotation of the parent.
        /// </summary>
        public virtual Vector3 EulerAngles { get => Entity.Transform.RotationEulerXYZ; set => Entity.Transform.RotationEulerXYZ = value; }
    }
}
