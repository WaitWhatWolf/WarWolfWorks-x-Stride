using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarWolfWorks_x_Stride.Interfaces;
using WarWolfWorks_x_Stride.Interfaces.NyuEntities;

namespace WarWolfWorks_x_Stride.NyuEntities.MovementSystem
{
    public abstract class NyuMovement : AsyncScript, ILockable
    {
        #region Lockable Implementation 
        public bool Locked { get; private set; }
        public event Action<ILockable> OnLocked;
        public event Action<ILockable> OnUnlocked;
        public void SetLock(bool to)
        {
            throw new NotImplementedException("To Do");
        }
        #endregion

        /// <summary>
        /// Invoked when a velocity is added.
        /// </summary>
        public event Action<IVelocity> OnVelocityAdded;
        /// <summary>
        /// Invoked when a velocity is removed.
        /// </summary>
        public event Action<IVelocity> OnVelocityRemoved;
       
        /// <summary>
        /// The default velocity to be applies to this <see cref="Nyu"/>. (<see cref="Vector3.zero"/> by default)
        /// </summary>
        public virtual Vector3 DefaultVelocity => Vector3.Zero;

        /// <summary>
        /// Every velocity stacked onto the entity's movement.
        /// </summary>
        public IEnumerable<IVelocity> Velocities => pv_Velocities;

        /// <summary>
        /// Adds a velocity to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(IVelocity velocity)
        {
            pv_Velocities.Add(velocity);

            velocity.Init(this);

            if (velocity is INyuTaskExecutable executable)
            {
                executable.SetTaskRunning(this, true);
                pv_VelocityTasks.Add(executable);
            }

            OnVelocityAdded?.Invoke(velocity);
        }

        /// <summary>
        /// Adds a list of velocities to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(params IVelocity[] velocities)
        {
            for (int i = 0; i < velocities.Length; i++)
            {
                AddVelocity(velocities[i]);
            }
        }

        /// <summary>
        /// Adds an enumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(IEnumerable<IVelocity> velocities)
        {
            foreach (IVelocity velocity in velocities)
            {
                AddVelocity(velocity);
            }
        }

        /// <summary>
        /// Removes an existing velocity.
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public bool RemoveVelocity(IVelocity velocity)
        {
            if (pv_Velocities.Remove(velocity))
            {
                if (velocity is INyuTaskExecutable executable)
                {
                    executable.SetTaskRunning(this, false);
                    pv_VelocityTasks.Remove(executable);
                }

                OnVelocityRemoved?.Invoke(velocity);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a list of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        /// <returns></returns>
        public bool[] RemoveVelocities(params IVelocity[] velocities)
        {
            bool[] toReturn = new bool[velocities.Length];

            for (int i = 0; i < velocities.Length; i++)
            {
                toReturn[i] = RemoveVelocity(velocities[i]);
            }

            return toReturn;
        }

        /// <summary>
        /// Removes a IEnumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        /// <returns></returns>
        public bool[] RemoveVelocities(IEnumerable<IVelocity> velocities)
        {
            IVelocity[] used = velocities.ToArray();

            return RemoveVelocities(used);
        }

        /// <summary>
        /// Returns true if the given velocity is contained within this <see cref="NyuMovement"/>.
        /// </summary>
        /// <returns></returns>
        public bool ContainsVelocity(IVelocity velocity)
        {
            return pv_Velocities.Contains(velocity);
        }

        /// <summary>
        /// Returns all velocities of given generic type.
        /// </summary>
        public List<T> GetAllVelocities<T>() where T : IVelocity
        {
            List<T> toReturn = new List<T>();
            foreach (IVelocity velocity in pv_Velocities)
            {
                if (velocity is T asT)
                    toReturn.Add(asT);
            }

            return toReturn;
        }

        /// <summary>
        /// Finds a velocity.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public IVelocity FindVelocity(Predicate<IVelocity> match)
            => pv_Velocities.Find(match);

        /// <summary>
        /// Finds all velocities that match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<IVelocity> FindAllVelocities(Predicate<IVelocity> match)
            => pv_Velocities.FindAll(match);

        /// <summary>
        /// Removes all velocities matching the given predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int RemoveAllVelocities(Predicate<IVelocity> match)
            => pv_Velocities.RemoveAll(match);

        /// <summary>
        /// Final Velocity that will be applied to this entity.
        /// </summary>
        public virtual Vector3 UsedVelocity
        {
            get
            {
                if (Locked)
                    return default;

                Vector3 toReturn = DefaultVelocity;

                for (int i = 0; i < pv_Velocities.Count; i++)
                {
                    try
                    {
                        Vector3 toAdd = pv_Velocities[i].GetValue();
                        toReturn += toAdd;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Skipped the calculation of a velocity as it generated an exception.", e);
                    }
                }

                return toReturn;
            }
        }


        /// <summary>
        /// Moves the <see cref="Nyu"/> to the given position.
        /// </summary>
        /// <param name="position">World position to move the <see cref="Nyu"/> to.</param>
        /// <param name="respectPhysics">If true, this method will try to respect the world physics in your game. (E.G not move inside another collider but next to it, etc..)</param>
        public abstract void MovePosition(Vector3 position, bool respectPhysics);

        public override async Task Execute()
        {
            while(Game.IsRunning)
            {
                await Script.NextFrame();
            }
        }

        private readonly List<INyuTaskExecutable> pv_VelocityTasks = new();

        private readonly List<IVelocity> pv_Velocities = new();
    }
}
