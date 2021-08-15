﻿using Stride.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using WarWolfWorks_x_Stride.Interfaces.NyuEntities;

namespace WarWolfWorks_x_Stride.NyuEntities.Statistics
{
    /// <summary>
    /// The class handling all statistics of an entity.
    /// </summary>
    public sealed class Stats : INyuReferencable
    {
        /// <summary>
        /// The parent entity.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

        /// <summary>
        /// Pointer to <see cref="Stacking.CalculatedValue(INyuStat)"/>.
        /// </summary>
        /// <param name="BaseStat"></param>
        /// <returns></returns>
        public float CalculatedValue(INyuStat BaseStat)
            => Stacking.CalculatedValue(BaseStat);

        /// <summary>
        /// Creates a temporary stat and uses it to retrieve the calculated value (Temporary Stat has a default stacking of 0).
        /// </summary>
        /// <param name="original"></param>
        /// <param name="affections"></param>
        /// <returns></returns>
        public float CalculatedValue(float original, int[] affections)
        {
            Stat toUse = new(original, 0, affections);
                return CalculatedValue(toUse);
        }

        /// <summary>
        /// Pointer to <see cref="CacheableStacking"/>'s <see cref="INyuCacheableStacking.RefreshStatCache(INyuStat)"/>; 
        /// Checks for null reference before accessing the cachable stacking.
        /// </summary>
        /// <param name="stat"></param>
        public void RefreshStatCache(INyuStat stat)
        {
            if (pv_CachableStackingExists)
                pv_CachableStacking.RefreshStatCache(stat);
        }

        /// <summary>
        /// Pointer to <see cref="CacheableStacking"/>'s <see cref="INyuCacheableStacking.ClearStatsCache()"/>;
        /// Checks for null reference before accessing the cachable stacking.
        /// </summary>
        public void ClearStatsCache()
        {
            if (pv_CachableStackingExists)
                pv_CachableStacking.ClearStatsCache();
        }

        /// <summary>
        /// The object which calculates all stats to return a final value.
        /// </summary>
        public INyuStacking Stacking => pv_Stacking;

        /// <summary>
        /// If the current stacking implements a <see cref="INyuCacheableStacking"/>, it will return it as such.
        /// </summary>
        public INyuCacheableStacking CacheableStacking => pv_CachableStacking;

        /// <summary>
        /// Attempts to set a new <see cref="INyuStacking"/>.
        /// </summary>
        /// <param name="ofType">The type of the <see cref="INyuStacking"/>.</param>
        /// <param name="args">Args to pass to the constructor of the given nyu stacking type.</param>
        public bool SetStacking(Type ofType, params object[] args)
        {
            try
            {
                if (typeof(INyuStacking).IsAssignableFrom(ofType))
                {
                    object[] pass = args == null || args.Length == 0 ? new object[] { this } : new object[] { this, args };
                    pv_Stacking = Activator.CreateInstance(ofType, pass) as INyuStacking;
                    TrySetCachableStacking();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(string.Empty, e);
                return false;
            }
        }

        /// <summary>
        /// Attempts to set a new <see cref="INyuStacking"/> based on a given generic type.
        /// </summary>
        /// <typeparam name="T">Type of stacking.</typeparam>
        /// <param name="args">Args passed to the constructor.</param>
        /// <returns></returns>
        public bool SetStacking<T>(params object[] args) where T : INyuStacking
        {
            try
            {
                object[] pass = args == null || args.Length == 0 ? new object[] { this } : new object[] { this, args };
                pv_Stacking = Activator.CreateInstance(typeof(T), pass) as INyuStacking;
                TrySetCachableStacking();
                return true;
            }
            catch(Exception e)
            {
                Log.Error(string.Empty, e);
                return false;
            }
        }

        /// <summary>
        /// Invoked when a stat is added.
        /// </summary>
        public event Action<INyuStat> OnStatAdded;

        /// <summary>
        /// Invoked when a stat is removed.
        /// </summary>
        public event Action<INyuStat> OnStatRemoved;

        /// <summary>
        /// Gets all stats returned in an array.
        /// </summary>
        /// <returns></returns>
        public INyuStat[] GetAllStats() => in_Stats.ToArray();

        /// <summary>
        /// Adds an <see cref="INyuStat"/> to stats to be calculated.
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddStat(INyuStat toAdd)
        {
            in_Stats.Add(toAdd);
            toAdd.OnAdded(this);

            if (toAdd is INyuTaskExecutable executable)
                executable.SetTaskRunning(NyuMain, true);

            OnStatAdded?.Invoke(toAdd);
        }
        /// <summary>
        /// Removes the given <see cref="INyuStat"/> from stats.
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveStat(INyuStat toRemove)
        {
            in_Stats.Remove(toRemove);

            if (toRemove is INyuTaskExecutable executable)
                executable.SetTaskRunning(NyuMain, false);

            OnStatRemoved?.Invoke(toRemove);
        }
        /// <summary>
        /// Adds a range of stats to calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void AddStats(IEnumerable<INyuStat> stats)
        {
            foreach (INyuStat stat in stats)
            {
                in_Stats.Add(stat);

                if (OnStatAdded == null)
                    continue;

                stat.OnAdded(this);
                OnStatAdded.Invoke(stat);
            }
        }
        /// <summary>
        /// Removes a range of stats from calculated stats.
        /// </summary>
        /// <param name="stats"></param>
        public void RemoveStats(IEnumerable<INyuStat> stats)
        {
            foreach (INyuStat stat in stats)
            {
                if (in_Stats.Remove(stat))
                {
                    if (OnStatRemoved == null)
                        return;

                    stat.OnRemoved(this);
                    OnStatRemoved.Invoke(stat);
                }
            }
        }

        /// <summary>
        /// Returns true if the given <see cref="INyuStat"/> was inside the calculated stats list.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public bool Contains(INyuStat stat) => in_Stats.Contains(stat);

        internal Stats(Nyu entity, ILogger log)
        {
            NyuMain = entity;
            Log = log;
            if (Stacking == null) SetStacking<DefaultStacking>(null);
        }

        private void TrySetCachableStacking()
        {
            pv_CachableStacking = null;
            pv_CachableStackingExists = false;

            if(pv_Stacking is INyuCacheableStacking cachable)
            {
                pv_CachableStacking = cachable;
                pv_CachableStackingExists = true;
            }
        }

        internal HashSet<INyuStat> in_Stats = new();
        private INyuStacking pv_Stacking;
        private INyuCacheableStacking pv_CachableStacking;
        private bool pv_CachableStackingExists;
        private readonly ILogger Log;
    }
}