using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static WarWolfWorksxS.WWWResources;

namespace WarWolfWorksxS.Utility
{
    /// <summary>
    /// Extension methods used for QOL; Most come from base methods in <see cref="Hooks"/>.
    /// </summary>
    public static class Extensions
    {
        #region Math
        /// <summary>
        /// Extension pointing to <see cref="Hooks.MathF.EulerAngles(Quaternion)"/>.
        /// </summary>
        /// <param name="quaternion">Rotation in Quaternion.</param>
        /// <returns></returns>
        public static Vector3 EulerAngles(this Quaternion quaternion)
        {
            return Hooks.MathF.EulerAngles(quaternion);
        }

        /// <summary>
        /// Extension pointing to <see cref="Hooks.MathF.Euler(Vector3)"/>.
        /// </summary>
        /// <param name="eulerAngles">Rotation in Vector3 space.</param>
        /// <returns></returns>
        public static Quaternion Euler(this Vector3 eulerAngles)
        {
            return Hooks.MathF.Euler(eulerAngles);
        }
        #endregion

        #region Random
        /// <summary>
        /// Returns a random float between 0f and 1f.
        /// </summary>
        public static float Next(this System.Random random)
            => (float)random.NextDouble();

        /// <summary>
        /// Returns a random float between the two given values <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        public static float Range(this System.Random random, float left, float right)
        {
            float min = Math.Min(left, right);
            float max = Math.Max(left, right);

            return (float)random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random float between the two given values <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        public static int Range(this System.Random random, int left, int right)
        {
            int min = Math.Min(left, right);
            int max = Math.Max(left, right);

            return random.Next(min, max);
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Puts a string value into a rainbow text using Unity's Rich Text format.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="frequency"></param>
        /// <param name="colorsToUse"></param>
        /// <returns></returns>
        public static string ToRainbow(this string original, int frequency, Color[] colorsToUse)
            => Hooks.Text.ToRainbow(original, frequency, colorsToUse);
        /// <summary>
        /// Extention method for <see cref="Hooks.Text.Cutout(string, string, string)"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Cutout(this string original, string from, string to)
            => Hooks.Text.Cutout(original, from, to);
        /// <summary>
        /// Extention method for <see cref="Hooks.Text.Cutout(string, char, char)"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Cutout(this string original, char from, char to)
           => Hooks.Text.Cutout(original, from, to);
        /// <summary>
        /// Returns true if numberToApproximate is not lower than or higer than numberToCompare using approximation as range.
        /// </summary>
        /// <param name="NumberToApproximate"></param>
        /// <param name="NumberToCompare"></param>
        /// <param name="approximation"></param>
        /// <returns></returns>
        public static bool IsApproximate(this float NumberToApproximate, float NumberToCompare, float approximation)
            => Hooks.MathF.IsApproximate(NumberToApproximate, NumberToCompare, approximation);
        /// <summary>
        /// Cuts a float to digits length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float Truncate(this float value, int digits) => Hooks.MathF.Truncate(value, digits);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToPositive(this float value) => Hooks.MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToNegative(this float value) => Hooks.MathF.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToPositive(this int value) => Hooks.MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToNegative(this int value) => Hooks.MathF.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToPositive(this long value) => Hooks.MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToNegative(this long value) => Hooks.MathF.ToNegative(value);
        /// <summary>
        /// Puts all values of a given color into negatives. (If a value was negative, it will be put into positive)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToNegative(this Color value) => Hooks.Colors.ToNegative(value);
        /// <summary>
        /// Returns true if the given position is within the bounds given. (<see cref="Vector4"/> bounds: X = minX, Y = minY, z = maxX, W = maxY)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector4 bounds)
            => Hooks.MathF.IsInsideBounds(position, bounds);
        /// <summary>
        /// Returns true if the given position is within the bounds given.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector2 min, Vector2 max)
            => Hooks.MathF.IsInsideBounds(position, min, max);
        /// <summary>
        /// <see cref="Hooks.Enumeration.Find{T}(T[], Predicate{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toUse"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T Find<T>(this T[] toUse, Predicate<T> match)
            => Array.Find(toUse, match);
        /// <summary>
        /// <see cref="Hooks.Enumeration.FindIndex{T}(T[], Predicate{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toUse"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] toUse, Predicate<T> match)
            => Array.FindIndex(toUse, match);
        /// <summary>
        /// <see cref="Hooks.Enumeration.ForEach{T}(T[], Action{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this T[] array, Action<T> action)
            => Array.ForEach(array, action);
        /// <summary>
        /// Returns a list with all null elements removed (if any)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveNull<T>(this IEnumerable<T> list)
        {
            return list.Where(t => t != null);
        }
        /// <summary>
        /// Returns true if the given <see cref="IEnumerable{T}"/> has toLookfor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="toLookfor"></param>
        /// <returns></returns>
        public static bool EnumerableContains<T>(this IEnumerable<T> list, T toLookfor)
        {
            if (list.Count() < 1)
            {
                return false;
            }
            foreach (T item in list)
            {
                if (item.Equals(toLookfor))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns an item from the given collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="toLookFor"></param>
        /// <returns></returns>
        public static T ReturnFromEnumerable<T>(this IEnumerable<T> list, T toLookFor)
        {
            foreach (T item in list)
            {
                if (item.Equals(toLookFor))
                {
                    return item;
                }
            }
            return default;
        }
        /// <summary>
        /// Itterates through each element <typeparamref name="T"/>, calls it's <typeparamref name="T"/>.ToString()
        /// and returns all of them in a string array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string[] EnumerableToString<T>(this IEnumerable<T> collection)
        {
            List<string> list = new List<string>();
            foreach (T item in collection)
            {
                list.Add(item.ToString());
            }
            return list.ToArray();
        }
        /// <summary>
        /// Returns the first instance of a null item inside a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetEmptyItem<T>(this IEnumerable<T> collection) where T : class
        {
            foreach (T item in collection)
            {
                if (item == null)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this List<T> list)
         => list.FindIndex(t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array)
        => array.FindIndex(t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, int index)
            => Array.FindIndex(array, index, t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index up to count times upwards in the enumerator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, int index, int count)
            => Array.FindIndex(array, index, count, t => t == null);
        /// <summary>
        /// Returns index of the first element T equal to null starting from IntRange.Min to IntRange.Max.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, IntRange range)
            => Array.FindIndex(array, range.Min, range.Max - range.Min, t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this IEnumerable<T> collection)
        {
            try
            {
                T[] array = collection.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    T val = array[i];
                    if (val == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// Returns the index of the first element equal to <typeparamref name="T"/> given.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public static int GetItemIndex<T>(this IEnumerable<T> collection, T Item) where T : class
        {
            List<T> list = new List<T>(collection);
            for (int i = 0; i < collection.Count(); i++)
            {
                if (list[i] != null && list[i].Equals(Item))
                {
                    return i;
                }
                if (list[i] == null && Item == null)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// Returns a new <see cref="ICollection{T}"/> of given size if collection passed was null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ICollection<T> SetCollectionSizeIfNull<T>(this ICollection<T> collection, int size)
        {
            if (collection == null || collection.Count == 0)
            {
                return new T[size];
            }
            return collection;
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and puts them as a <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Queue<T> ToQueueSized<T>(this ICollection<T> collection, int size, bool fromEnd)
        {
            if (collection.Count <= 0)
            {
                return null;
            }
            int num = (collection.Count - 1 < size) ? (collection.Count - 1) : size;
            int index = fromEnd ? (collection.Count - 1 - num) : 0;
            List<T> range = collection.ToList().GetRange(index, num);
            return new Queue<T>(range);
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and puts them as a <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Stack<T> ToStackSized<T>(this ICollection<T> collection, int size, bool fromEnd)
        {
            if (collection.Count <= 0)
            {
                return null;
            }
            int num = (collection.Count - 1 < size) ? (collection.Count - 1) : size;
            int index = fromEnd ? (collection.Count - 1 - num) : 0;
            List<T> range = collection.ToList().GetRange(index, num);
            return new Stack<T>(range);
        }
        /// <summary>
        /// Gets all <typeparamref name="T"/> items inside a <see cref="ValueTuple{T1, T2}"/> based on item index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">If 0, it returns Item1, otherwise returns Item2.</param>
        /// <returns></returns>
        public static T[] GetItemsFromTupleIndex<T>(this ICollection<(T, T)> collection, int item)
        {
            List<T> list = new List<T>();
            foreach (var item2 in collection)
            {
                list.Add((item == 0) ? item2.Item1 : item2.Item2);
            }
            return list.ToArray();
        }
        /// <summary>
        /// Gets all <typeparamref name="T"/> items inside a <see cref="ValueTuple{T1, T2, T3}"/> based on item index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">If 0, it returns Item1; if 1, returns Item2; otherwise returns Item3.</param>
        /// <returns></returns>
        public static T[] GetItemsFromTupleIndex<T>(this ICollection<(T, T, T)> collection, int item)
        {
            List<T> list = new List<T>();
            foreach (var item3 in collection)
            {
                List<T> list2 = list;
                object item2;
                switch (item)
                {
                    default:
                        item2 = item3.Item3;
                        break;
                    case 1:
                        item2 = item3.Item2;
                        break;
                    case 0:
                        item2 = item3.Item1;
                        break;
                }
                list2.Add((T)item2);
            }
            return list.ToArray();
        }
        /// <summary>
        /// Removes an array of strings from original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="removers"></param>
        /// <completionlist cref="string"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        public static string RemoveArrayFromString(this string original, IEnumerable<string> removers)
        {
            if (removers == null)
            {
                goto Returner;
            }
            foreach (string remover in removers)
            {
                original = original.Replace(remover, string.Empty);
            }
            Returner:
            return original;
        }

        /// <summary>
        ///  Creates a rotation from center position to destination using Atan2.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="destination"></param>
        /// <param name="reversed"></param>
        /// <param name="adder">Z rotation to add onto the result.</param>
        /// <returns></returns>
        public static Quaternion RotateTowards2D(this Vector2 center, Vector2 destination, bool reversed, float adder)
        {
            Vector3 used = new Vector3(center - destination, 0);
            float num = MathF.Atan2(used.Y, used.X) * Hooks.MathF.Rad2Deg;
            if (reversed)
            {
                num += 180f;
            }
            Vector3 euler = new Vector3(0f, 0f, num + adder);
            return Euler(euler);
        }

        /// <summary>
        /// Loads a sprite from a spritesheet with the given info.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="SpriteSheetName"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite LoadSpriteFromSpriteSheet(this ContentManager content, string SpriteSheetName, string spriteName)
            => Hooks.LoadSpriteFromSpriteSheet(content, SpriteSheetName, spriteName);

        /// <summary>
        /// Equivalent to ASCII <see cref="Encoding.GetBytes(char[])"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        /// <summary>
        /// Equivalent to ASCII <see cref="Encoding.GetString(byte[])"/>.
        /// </summary>
        /// <param name="byt"></param>
        /// <returns></returns>
        public static string ToStringFromBytes(this byte[] byt)
        {
            return Encoding.ASCII.GetString(byt);
        }
        /// <summary>
        /// Returns an attribute value based on type and func given.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="type"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default;
        }
        #endregion

    }
}
