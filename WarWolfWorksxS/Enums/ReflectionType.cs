using System;
using WarWolfWorksxS.Utility;

namespace WarWolfWorksxS.Enums
{
    /// <summary>
    /// Used by <see cref="Hooks.Reflection"/> to determine what type of value to search for.
    /// </summary>
    [Flags]
    public enum ReflectionType
    {
        Field = 4,
        Property = 8,
        Method = Property << 1,
    }
}
