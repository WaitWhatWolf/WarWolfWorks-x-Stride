using System;
using WarWolfWorks_x_Stride.NyuEntities;

namespace WarWolfWorks_x_Stride
{
    /// <summary>
    /// All static & constant resources of this class library.
    /// </summary>
    public static class WWWResources
    {
        /// <summary>
        /// Resources related to <see cref="Nyu"/> entities.
        /// </summary>
        public static class NyuEntities
        {
            #region Stacking
            /// <summary>
            /// Base stacking index of the default stacking calculation. Stats with this stacking will be used as the base value for calculation.
            /// </summary>
            public const int STATS_STACKING_BASE = -1;
            /// <summary>
            /// Overrider stacking index of the default stacking calculation. Stats with this stacking will override the base value with it's own. (useful for weapons or items if they have base stats)
            /// Calculation: Base = Overrider
            /// </summary>
            public const int STATS_STACKING_OVERRIDER = 0;
            /// <summary>
            /// Additive stacking index of the default stacking calculation. Stats with this stacking will add themselves on top of the base value.
            /// Calculation: Base + Value
            /// </summary>
            public const int STATS_STACKING_ADDITIVE = 1;
            /// <summary>
            /// Base Multiplier stacking index of the default stacking calculation. Stats with this stacking will multiply the BASE value by their own.
            /// Calculation: Base * (Value + 1)
            /// </summary>
            public const int STATS_STACKING_BASEMULT = 2;
            /// <summary>
            /// Total Multiplier stacking index of the default stacking calculation. Stats with this stacking will multiply the TOTAL value by their own.
            /// Calculation: (Base + All Value Calculations) * (Value + 1)
            /// </summary>
            public const int STATS_STACKING_TOTALMULT = 3;
            /// <summary>
            /// Pwner stacking index of the default stacking calculation. Stats with this stacking will ignore any calculation and return their own value.
            /// Calculation: (Base + All Value Calculation) = Value
            /// </summary>
            public const int STATS_STACKING_PWNER = 42;
            #endregion
        }
    }
}
