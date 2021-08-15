using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Class for a parenting system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentable<T>
    {
        /// <summary>
        /// The parent.
        /// </summary>
        T Parent { get; }
    }
}
