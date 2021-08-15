using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarWolfWorks_x_Stride.Interfaces
{
    /// <summary>
    /// Interface used for "locking" an object.
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Determimes if the current object is locked.
        /// </summary>
        public bool Locked { get; }
        /// <summary>
        /// Sets the lock.
        /// </summary>
        /// <param name="to"></param>
        public void SetLock(bool to);
        /// <summary>
        /// Called when SetLock was called as true.
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when SetLock was called as false.
        /// </summary>
        public event Action<ILockable> OnUnlocked;
    }
}
