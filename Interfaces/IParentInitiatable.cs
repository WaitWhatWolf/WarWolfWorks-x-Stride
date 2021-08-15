using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Used for basic initiation of a child object.
    /// </summary>
    public interface IParentInitiatable<T> : IInitiated, IParentable<T>
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        bool Init(T parent);
    }
}
