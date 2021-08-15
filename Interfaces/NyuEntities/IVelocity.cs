using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarWolfWorks_x_Stride.Interfaces.NyuEntities
{
    public interface IVelocity : IParentInitiatable<NyuMovement>, INyuReferencable
    {
        /// <summary>
        /// The final value applied to this velocity.
        /// </summary>
        /// <returns></returns>
        Vector3 GetValue();
    }
}
