﻿using Stride.Core.Mathematics;

namespace WarWolfWorksxS.Interfaces
{
    /// <summary>
    /// Indicates an object which has a position.
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// The position.
        /// </summary>
        Vector3 Position { get; set; }
    }
}
