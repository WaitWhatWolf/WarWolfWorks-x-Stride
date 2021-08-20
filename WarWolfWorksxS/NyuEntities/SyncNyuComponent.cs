using Stride.Core.Mathematics;
using Stride.Engine;
using WarWolfWorksxS.Interfaces;
using WarWolfWorksxS.Interfaces.NyuEntities;
using WarWolfWorksxS.Internal;

namespace WarWolfWorksxS.NyuEntities
{
    /// <summary>
    /// A <see cref="INyuComponent"/> implementation using an <see cref="SyncScript"/> class.
    /// </summary>
    [RequireComponent(typeof(Nyu))]
    public abstract class SyncNyuComponent : SyncScript, INyuComponent, INyuObject, IRotation, IPosition, IEulerAngles
    {
        /// <summary>
        /// The owner of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

        /// <summary>
        /// Shorthand for Entity.Transform
        /// </summary>
        public TransformComponent Transform => Entity.Transform;

        /// <summary>
        /// The position of the parent.
        /// </summary>
        public Vector3 Position { get => NyuMain.Position; set => NyuMain.Position = value; }

        /// <summary>
        /// The rotation of the parent.
        /// </summary>
        public Quaternion Rotation { get => NyuMain.Rotation; set => NyuMain.Rotation = value; }

        /// <summary>
        /// The euler rotation of the parent.
        /// </summary>
        public Vector3 EulerAngles { get => NyuMain.EulerAngles; set => NyuMain.EulerAngles = value; }

        /// <summary>
        /// Pointer to <see cref="ScriptComponent.Game"/> cast as <see cref="Internal.WWWGame"/>.
        /// </summary>
        public WWWGame WWWGame => (WWWGame)Game;

        /// <summary>
        /// Sets <see cref="NyuMain"/>; When overriding, make sure to include "base.Start();".
        /// </summary>
        public override void Start()
        {
            NyuMain = Entity.Get<Nyu>();
        }
    }
}
