using Stride.Core.Mathematics;

namespace WarWolfWorksxS.Interfaces
{
    /// <summary>
    /// Indicates an object which has euler angles.
    /// </summary>
    interface IEulerAngles
    {
        /// <summary>
        /// The euler angles.
        /// </summary>
        Vector3 EulerAngles { get; set; }
    }
}
