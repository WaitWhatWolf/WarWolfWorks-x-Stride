using Stride.Core.Mathematics;
using Stride.Engine;
using WarWolfWorksxS.NyuEntities.Statistics;
using WarWolfWorksxS.Interfaces.NyuEntities;
using System.Threading.Tasks;
using WarWolfWorksxS.Internal;

namespace WarWolfWorksxS.NyuEntities
{
    /// <summary>
    /// Core class to identify Nyu entities by.
    /// </summary>
    public abstract class Nyu : AsyncScript, INyuObject
    {
        /// <summary>
        /// The entity's stats manager.
        /// </summary>
        public Stats Stats { get; internal set; }

        /// <summary>
        /// Shorthand for Entity.Transform
        /// </summary>
        public TransformComponent Transform => Entity.Transform;

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

        /// <summary>
        /// Pointer to <see cref="ScriptComponent.Game"/> cast as <see cref="Internal.WWWGame"/>.
        /// </summary>
        public WWWGame WWWGame => (WWWGame)Game;

        /// <summary>
        /// Initializes <see cref="Stats"/> and returns <see cref="Task.CompletedTask"/>;
        /// When overriding, make sure to include "await base.Execute();"
        /// </summary>
        /// <returns></returns>
        public override Task Execute()
        {
            Stats = new Stats(this);
            return Task.CompletedTask;
        }
    }
}
