using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarWolfWorks_x_Stride.Interfaces.NyuEntities;

namespace WarWolfWorks_x_Stride.NyuEntities
{
    public abstract class NyuComponent : AsyncScript, INyuReferencable
    {
        /// <summary>
        /// The owner of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

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
    }
}
