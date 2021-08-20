using Microsoft.Win32;
using Stride.Core.Extensions;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Graphics;
using Stride.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WarWolfWorksxS.Enums;
using WarWolfWorksxS.Interfaces;
using static WarWolfWorksxS.WWWResources.Streaming;

namespace WarWolfWorksxS.Utility
{
    /// <summary>
    /// A class which contains 20+Gadzillion-billion-yes methods for various utilities.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        /// Subclass with all reflection methods.
        /// </summary>
        public static class Reflection
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T">Where should the reflection be performed?</typeparam>
            /// <typeparam name="TKey">Key value of the dictionary.</typeparam>
            /// <typeparam name="TValue">Value value of the dictionary.</typeparam>
            /// <param name="owner">Who is assigned to get the object off of.</param>
            /// <param name="fieldName">Name of the dictionary.</param>
            /// <param name="value">Value to be added to the dictionary.</param>
            /// <param name="type">Which kind of reflection should be performed. (Note: only takes <see cref="ReflectionType.Field"/> and <see cref="ReflectionType.Property"/> as valid types; Any other type is ignored)</param>
            public static void AddToDictionary<T, TKey, TValue>(object owner, string fieldName, KeyValuePair<TKey, TValue> value, ReflectionType type)
            {
                Dictionary<TKey, TValue> dic;

                switch (type)
                {
                    default: return;
                    case ReflectionType.Field:
                        dic = (Dictionary<TKey, TValue>)typeof(T).GetField(fieldName).GetValue(owner);
                        break;
                    case ReflectionType.Property:
                        dic = (Dictionary<TKey, TValue>)typeof(T).GetProperty(fieldName).GetValue(owner);
                        break;
                }

                dic.Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Subclass with all utility methods for use of a random factor.
        /// </summary>
        public static class Random
        {
            /// <summary>
            /// The core <see cref="System.Random"/> class used for all <see cref="Random"/> methods.
            /// You can provide your own random value, keep in mind however that null values are ignored.
            /// </summary>
            public static System.Random Core
            {
                get => pv_Core;
                set
                {
                    if (value is not null)
                        pv_Core = value;
                }
            }

            /// <summary>
            /// Returns anything between Vector3(-1f, -1f) and Vector3(1f, 1f).
            /// </summary>
            public static Vector3 RandomVector01WithNeg => new(RangeUnsafe(-1f, 1f), RangeUnsafe(-1f, 1f), RangeUnsafe(-1f, 1f));

            /// <summary>
            /// Returns anything between <see cref="Vector3.Zero"/> and <see cref="Vector3.One"/>
            /// </summary>
            public static Vector3 RandomVector01 => new(RangeUnsafe(0f, 1f), RangeUnsafe(0f, 1f), RangeUnsafe(0f, 1f));

            /// <summary>
            /// Returns a random float value; Uses unsafe code.
            /// </summary>
            /// <returns></returns>
            public static unsafe float NextUnsafe()
            {
                var sign = Core.Next(2);
                var exponent = Core.Next((1 << 8) - 1); // do not generate 0xFF (infinities and NaN)
                var mantissa = Core.Next(1 << 23);

                var bits = (sign << 31) + (exponent << 23) + mantissa;
                return IntBitsToFloat(bits);
            }

            /// <summary>
            /// Generates a random float between 0f and 1f.
            /// </summary>
            /// <returns></returns>
            public static float Next() => (float)Core.NextDouble();

            /// <summary>
            /// Returns a random float value between left and right; Uses unsafe code. (min-max values are automatically detected, so order is not relevant)
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static float RangeUnsafe(float left, float right)
            {
                float min = Math.Min(left, right);
                float max = Math.Max(left, right);

                return (NextUnsafe() % (max - min)) + min;
            }

            /// <summary>
            /// Returns a random float value between left and right. (min-max values are automatically detected, so order is not relevant)
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static float Range(float left, float right)
            {
                float min = Math.Min(left, right);
                float max = Math.Max(left, right);

                return Next() * (max - min) + min;
            }

            /// <summary>
            /// Returns a random int value between left and right. (min-max values are automatically detected, so order is not relevant)
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static int Range(int left, int right)
            {
                int min = System.Math.Min(left, right);
                int max = System.Math.Max(left, right);

                return Core.Next(min, max);
            }

            /// <summary>
            /// Used by <see cref="NextUnsafe"/>.
            /// </summary>
            /// <param name="bits"></param>
            /// <returns></returns>
            private static float IntBitsToFloat(int bits)
            {
                unsafe
                {
                    return *(float*)&bits;
                }
            }

            /// <summary>
            /// Shuffles all items inside a list, changing their index.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static List<T> Shuffle<T>(List<T> list)
            {
                int num = list.Count;
                while (num > 1)
                {
                    num--;
                    int index = Range(0, num + 1);
                    T value = list[index];
                    list[index] = list[num];
                    list[num] = value;
                }
                return list;
            }

            /// <summary>
            /// Returns a random item from an IEnumerable value.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="List"></param>
            /// <returns></returns>
            public static T RandomItem<T>(IEnumerable<T> List)
            {
                T[] array = List.ToArray();
                int num = Range(0, array.Length);
                return array[num];
            }

            /// <summary>
            /// Returns a <see cref="Vector3"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector3 Range(Vector3 min, Vector3 max)
            {
                return new Vector3(RangeUnsafe(min.X, max.X), RangeUnsafe(min.Y, max.Y), RangeUnsafe(min.Z, max.Z));
            }

            /// <summary>
            /// Returns a <see cref="Vector2"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector2 Range(Vector2 min, Vector2 max)
            {
                return new Vector2(RangeUnsafe(min.X, max.X), RangeUnsafe(min.Y, max.Y));
            }

            /// <summary>
            /// Returns a random string with characters between a-z, A-Z and 0-9.
            /// </summary>
            /// <param name="length"></param>
            /// <returns></returns>
            public static string GetRandomString(int length)
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[Range(0, s.Length)]).ToArray());
            }

            private static System.Random pv_Core = new();
        }

        /// <summary>
        /// Subclass with all streaming and saving/loading methods.
        /// </summary>
        public static class Streaming
        {
            /// <summary>
            /// Attempts to create a folder. Returns true if the folder was successfully created.
            /// </summary>
            /// <param name="folderPath"></param>
            /// <returns></returns>
            public static bool CreateFolder(string folderPath)
            {
                if (Enumerable.Contains(folderPath, '.'))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
                bool flag = Directory.Exists(folderPath);
                if (!flag)
                {
                    Directory.CreateDirectory(folderPath);
                }
                return !flag;
            }

            /// <summary>
            /// Gets all files inside a given folder.
            /// </summary>
            /// <param name="folderPath"></param>
            /// <param name="extention"></param>
            /// <param name="includeFolderPath"></param>
            /// <returns></returns>
            public static string[] GetAllFilesInFolder(string folderPath, string extention, bool includeFolderPath)
            {
                string actFolderPath = Path.GetDirectoryName(folderPath);
                if (!Directory.Exists(actFolderPath))
                {
                    return null;
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(actFolderPath);
                FileInfo[] files = directoryInfo.GetFiles(extention);
                List<string> list = new List<string>();
                FileInfo[] array = files;
                foreach (FileInfo fileInfo in array)
                {
                    list.Add(includeFolderPath ? (actFolderPath + fileInfo.Name) : fileInfo.Name);
                }
                return list.ToArray();
            }

            private static bool InternalEncryptionFile(string path, string password, bool encrypt)
            {
                try
                {
                    string VanillaText = File.ReadAllText(path);
                    string Encryption = encrypt ? RijndaelEncryption.Encrypt(VanillaText, password) : RijndaelEncryption.Decrypt(VanillaText, password);
                    if (VanillaText.Equals(Encryption))
                        return false;

                    StringBuilder writer = new StringBuilder(Encryption);
                    if (encrypt)
                    {
                        for (int i = STREAMING_FILE_ENCRYPTION_JUMPER; i < writer.Length; i += STREAMING_FILE_ENCRYPTION_JUMPER)
                        {
                            writer.Insert(i, '\n');
                        }
                    }
                    else
                        writer.Replace("\n", string.Empty);

                    File.WriteAllText(path, writer.ToString());

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Encrypts all contents of a file using <see cref="RijndaelEncryption"/>.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static bool EncryptFile(string path, string password)
                => InternalEncryptionFile(path, password, true);

            /// <summary>
            /// Decrypts all contents of a file using <see cref="RijndaelEncryption"/>, assuming it was previously encrypted using <see cref="EncryptFile(string, string)"/>.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static bool DecryptFile(string path, string password)
            => InternalEncryptionFile(path, password, false);

            /// <summary>
            /// Gets all lines from a StreamReader and returns them as a string array. (THIS METHOD DOES NOT FLUSH OR DISPOSE THE STREAMREADER)
            /// </summary>
            /// <param name="sr"></param>
            /// <returns></returns>
            public static string[] GetAllStreamedLines(StreamReader sr)
            {
                List<string> list = new List<string>();
                string item;
                while ((item = sr.ReadLine()) != null)
                {
                    list.Add(item);
                }
                return list.ToArray();
            }

            /// <summary>
            /// Overrides a line at the given index of a file.
            /// </summary>
            /// <param name="newText"></param>
            /// <param name="filePath"></param>
            /// <param name="index"></param>
            public static void LineChanger(string filePath, int index, string newText)
            {
                string[] array = File.ReadAllLines(filePath);
                array[index - 1] = newText;
                File.WriteAllLines(filePath, array);
            }

            /// <summary>
            /// Returns the Assets folder path in windows form.
            /// </summary>
            [Obsolete("Currently not operational, returns string.Empty.")]
            public static string AssetsPath => string.Empty;
            /// <summary>
            /// Returns a merged path between <see cref="AssetsPath"/> and fileName given.
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            [Obsolete("Currently not operational, returns string.Empty.")]
            public static string GetAssetsFilePath(string fileName) => string.Empty;
        }
        
        internal static class EncryptionUtilityInternal
        {
            internal const int Keysize = 256;
            internal const int DerivationIterations = 1000;
            internal static byte[] Generate256BitsOfRandomEntropy()
            {
                byte[] randomBytes = new byte[32]; // 32 Bytes => 256 bits.
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }

            internal static string EncryptingKey(string value)
            {
                StringBuilder Sb = new StringBuilder();

                using (SHA256 hash = SHA256Managed.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));
                }

                return Sb.ToString();
            }
        }

        /// <summary>
        /// Encryption class which uses the Rijndael encryption algorithm.
        /// </summary>
        public static class RijndaelEncryption
        {
            private static string EncryptInternal(string input, string password, CipherMode mode, PaddingMode padding)
            {
                try
                {
                    password = EncryptionUtilityInternal.EncryptingKey(password);

                    byte[] saltStringBytes = EncryptionUtilityInternal.Generate256BitsOfRandomEntropy();
                    byte[] ivStringBytes = EncryptionUtilityInternal.Generate256BitsOfRandomEntropy();
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
                    using (Rfc2898DeriveBytes passwordIntern = new Rfc2898DeriveBytes(password, saltStringBytes, EncryptionUtilityInternal.DerivationIterations))
                    {
                        byte[] keyBytes = passwordIntern.GetBytes(EncryptionUtilityInternal.Keysize / 8);
                        using (RijndaelManaged symmetricKey = new RijndaelManaged())
                        {
                            symmetricKey.BlockSize = EncryptionUtilityInternal.Keysize;
                            symmetricKey.Mode = mode;
                            symmetricKey.Padding = padding;
                            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                            {
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                    {
                                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                        cryptoStream.FlushFinalBlock();
                                        byte[] cipherTextBytes = saltStringBytes;
                                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                        memoryStream.Close();
                                        cryptoStream.Close();
                                        return Convert.ToBase64String(cipherTextBytes);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(CryptographicException)
                {
                    return input;
                }
            }

            /// <summary>
            /// Returns an encrypted version of this string using the given password; CipherMode is set to CBC and PaddingMode to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password)
                => EncryptInternal(input, password, CipherMode.CBC, PaddingMode.PKCS7);

            /// <summary>
            /// Returns an encrypted version of this string using the given password; PaddingMode is set to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password, CipherMode mode)
                => EncryptInternal(input, password, mode, PaddingMode.PKCS7);

            /// <summary>
            /// Returns an encrypted version of this string using the given password.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password, CipherMode mode, PaddingMode padding)
               => EncryptInternal(input, password, mode, padding);

            private static string DecryptInternal(string input, string password, CipherMode mode, PaddingMode padding)
            {
                try
                {
                    password = EncryptionUtilityInternal.EncryptingKey(password);

                    byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(input);
                    byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(EncryptionUtilityInternal.Keysize / 8).ToArray();
                    byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(EncryptionUtilityInternal.Keysize / 8).Take(EncryptionUtilityInternal.Keysize / 8).ToArray();
                    byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((EncryptionUtilityInternal.Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((EncryptionUtilityInternal.Keysize / 8) * 2)).ToArray();

                    using (Rfc2898DeriveBytes passwordInternal = new Rfc2898DeriveBytes(password, saltStringBytes, EncryptionUtilityInternal.DerivationIterations))
                    {
                        byte[] keyBytes = passwordInternal.GetBytes(EncryptionUtilityInternal.Keysize / 8);
                        using (RijndaelManaged symmetricKey = new RijndaelManaged())
                        {
                            symmetricKey.BlockSize = EncryptionUtilityInternal.Keysize;
                            symmetricKey.Mode = mode;
                            symmetricKey.Padding = padding;
                            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                            {
                                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                                {
                                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                    {
                                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                        memoryStream.Close();
                                        cryptoStream.Close();
                                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(CryptographicException)
                {
                    return input;
                }
            }

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>; CipherMode is set to CBC and PaddingMode to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password)
                => DecryptInternal(input, password, CipherMode.CBC, PaddingMode.PKCS7);

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>; PaddingMode is set to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password, CipherMode mode)
                => DecryptInternal(input, password, mode, PaddingMode.PKCS7);

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password, CipherMode mode, PaddingMode padding)
                => DecryptInternal(input, password, mode, padding);
        }

        /// <summary>
        /// Cipher algorithms.
        /// </summary>
        public static class Ciphers
        {
            /// <summary>
            /// Ceasar cipher which delays each character in a string by shift.
            /// </summary>
            /// <param name="source"></param>
            /// <param name="shift"></param>
            /// <returns></returns>
            public static string Caesar(string source, int shift)
            {
                var maxChar = Convert.ToInt32(char.MaxValue);
                var minChar = Convert.ToInt32(char.MinValue);

                var buffer = source.ToCharArray();

                for (var i = 0; i < buffer.Length; i++)
                {
                    var shifted = Convert.ToInt32(buffer[i]) + shift;

                    if (shifted > maxChar)
                    {
                        shifted -= maxChar;
                    }
                    else if (shifted < minChar)
                    {
                        shifted += maxChar;
                    }

                    buffer[i] = Convert.ToChar(shifted);
                }

                return new string(buffer);
            }
        }

        /// <summary>
        /// Contains all methods concerning the mouse/cursor.
        /// </summary>
        public static class Cursor
        {
            /// <summary>
            /// Returns the anchored position of the mouse in Vector2. Pointer to <see cref="InputManager.MousePosition"/>.
            /// </summary>
            public static Vector2 MousePosInPercent(InputManager input) => input.MousePosition;
            /// <summary>
            /// Returns the absolute position of the mouse in pixels. Pointer to <see cref="InputManager.AbsoluteMousePosition"/>.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static Vector2 MousePos(InputManager input) => input.AbsoluteMousePosition;

            /// <summary>
            /// Returns a rotation from position based on the camera, which would rotate towards the mouse.
            /// </summary>
            /// <param name="camera"></param>
            /// <param name="input"></param>
            /// <param name="position"></param>
            /// <param name="adder"></param>
            /// <returns></returns>
            public static Quaternion RotateTowardsMouse(CameraComponent camera, InputManager input, Vector2 position, float adder = 0f)
            {
                Vector3 vector = GetMouseWorldPosition(camera, input);
                float z = System.MathF.Atan2(vector.Y - position.Y, vector.X - position.X) * MathF.Rad2Deg;
                Vector3 euler = new (0f, 0f, camera.Entity.Transform.RotationEulerXYZ.Z + z + adder);
                return MathF.Euler(euler);
            }

            /// <summary>
            /// Gets the world position of the mouse.
            /// </summary>
            /// <param name="camera"></param>
            /// <param name="input"></param>
            /// <returns></returns>
            public static Vector3 GetMouseWorldPosition(CameraComponent camera, InputManager input)
            {
                camera.Update();
                return new Vector3(MousePosInPercent(input).X * -camera.ProjectionMatrix.M31, MousePosInPercent(input).Y * -camera.ProjectionMatrix.M32, -camera.ProjectionMatrix.M33);
            }

        }

        /// <summary>
        /// Contains all Color utilities.
        /// </summary>
        public static class Colors
        {
            /// <summary>
            /// The best color. (Orange color with a red-ish hue)
            /// </summary>
            public static readonly Color Tangelo = new Color(0.976f, 0.302f, 0f);
            /// <summary>
            /// The Orange color.
            /// </summary>
            public static readonly Color Orange = new Color(1f, .647f, 0f);
            /// <summary>
            /// The Pink color.
            /// </summary>
            public static readonly Color Pink = new Color(1f, .753f, .796f);
            /// <summary>
            /// The Crimson color. (light red)
            /// </summary>
            public static readonly Color Crimson = new Color(.863f, .078f, .235f);
            /// <summary>
            /// The Crimson Red color. (dark red, not to be confused with <see cref="Crimson"/>)
            /// </summary>
            public static readonly Color CrimsonRed = new Color(.6f, 0f, 0f);
            /// <summary>
            /// The Midnight Blue color. (dark blue)
            /// </summary>
            public static readonly Color MidnightBlue = new Color(.098f, .098f, .439f);

            /// <summary>
            /// The Wisteria color. (dark purple)
            /// </summary>
            public static readonly Color Wisteria = new Color(.79f, .63f, .86f);
            /// <summary>
            /// The Cotton Candy color. (dark, high contrast pink)
            /// </summary>
            public static readonly Color CottonCandy = new Color(1f, .74f, .85f);
            /// <summary>
            /// The Daffodil color. (slightly darker yellow)
            /// </summary>
            public static readonly Color Daffodil = new Color(1f, 1f, .19f);
            /// <summary>
            /// The Azure color. (very light blue)
            /// </summary>
            public static readonly Color Azure = new Color(.94f, 1f, 1f);
            /// <summary>
            /// The Ao/Office Green color. (dark green)
            /// </summary>
            public static readonly Color Ao = new Color(0f, .5f, 0f);
            /// <summary>
            /// The Electric Ultramarine color. (high contrast purple)
            /// </summary>
            public static readonly Color ElectricUltramarine = new Color(.25f, 0f, 1f);
            /// <summary>
            /// The Ferrari Red color. (slightly orange red)
            /// </summary>
            public static readonly Color FerrariRed = new Color(1f, .16f, 0f);
            /// <summary>
            /// The Gold color. (high contrast yellow)
            /// </summary>
            public static readonly Color Gold = new Color(1f, .84f, 0f);
            /// <summary>
            /// The Lapis Lazuli color. (pale blue)
            /// </summary>
            public static readonly Color LapisLazuli = new Color(.15f, .38f, .61f);
            /// <summary>
            /// The Lawn Green color. (VERY high contract green)
            /// </summary>
            public static readonly Color LawnGreen = new Color(.49f, .99f, 0f);
            /// <summary>
            /// The Oxford Blue color. (very dark blue)
            /// </summary>
            public static readonly Color OxfordBlue = new Color(0f, .13f, .28f);
            /// <summary>
            /// The Psychedelic Purple/Phlox color. (slightly lighter <see cref="Color.magenta"/>)
            /// </summary>
            public static readonly Color PsychedelicPurple = new Color(.87f, 0f, 1f);
            /// <summary>
            /// The Royal Blue color. (slightly pale blue)
            /// </summary>
            public static readonly Color RoyalBlue = new Color(.25f, .41f, .88f);
            /// <summary>
            /// The Timberwolf color. (gray with a brown tint)
            /// </summary>
            public static readonly Color Timberwolf = new Color(.86f, .84f, .82f);
            /// <summary>
            /// The Zinnwaldite Brown color. (very dark brown)
            /// </summary>
            public static readonly Color ZinnwalditeBrown = new Color(.17f, .09f, .04f);

            /// <summary>
            /// Returns the Vector4.MoveTowards equivalent for colors.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="destination"></param>
            /// <param name="speed"></param>
            /// <returns></returns>
            public static Color MoveTowards(Color point, Color destination, float speed)
            {
                return new Color(MathF.MoveTowards(point.ToVector4(), destination.ToVector4(), speed));
            }

            /// <summary>
            /// Returns the original color with it's values being put into negatives. (If a value is negative, it will be put back to positive)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color ToNegative(Color original)
                => new Color(-original.R, -original.G, -original.B, -original.A);

            /// <summary>
            /// Returns the original color with it's values being put into negatives. (If a value is negative, it will be kept as is)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color ToAbsoluteNegative(Color original)
                => new Color(MathF.ToNegative(original.R), MathF.ToNegative(original.G), MathF.ToNegative(original.B), MathF.ToNegative(original.A));

            /// <summary>
            /// Short for <see cref="Color.white"/> - original.
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color Reverse(Color original) => Color.White - original;

            /// <summary>
            /// Returns in order: Color.red, Color.yellow, Color.blue.
            /// </summary>
            public static readonly Color[] PrimaryColors = new Color[]
            {
                Color.Red,
                Color.Yellow,
                Color.Blue
            };

            /// <summary>
            /// Returns in order: Color.red, Color.yellow, Color.cyan, Color.blue, Color.magenta
            /// </summary>
            public static readonly Color[] MainColors = new Color[6]
            {
                Color.Red,
                Color.Yellow,
                Color.Green,
                Color.Cyan,
                Color.Blue,
                Color.Magenta
            };

            /// <summary>
            /// Returns all colors of the rainbow in descending order.
            /// </summary>
            public static readonly Color[] RainbowColors = new Color[7]
            {
                new Color(1f, 0f, 0f),
                new Color(1f, 0.35f, 0f),
                new Color(1f, 1f, 0f),
                new Color(0f, 1f, 0f),
                new Color(0f, 1f, 1f),
                new Color(0f, 0.35f, 1f),
                new Color(1f, 0f, 1f)
            };

            /// <summary>
            /// Returns the hexcode value of a color.
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public static string ColorToHex(Color color)
            {
                return Stride.Core.Mathematics.ColorExtensions.RgbToString(color.ToRgba());
            }

            /// <summary>
            /// Returns the median of two given colors.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Color MiddleMan(Color a, Color b)
            {
                float r = a.R + b.R / 2f;
                float g = a.G + b.G / 2f;
                float b2 = a.B + b.B / 2f;
                float a2 = a.A + b.A / 2f;
                return new Color(r, g, b2, a2);
            }

            /// <summary>
            /// Returns the median of two given colors, where percentage 0 is a, 0.5 is the exact median, and 1 is b.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percentage"></param>
            /// <returns></returns>
            public static Color MiddleMan(Color a, Color b, float percentage)
            {
                float num = 1f - percentage;
                float num2 = a.R * num;
                float num3 = a.G * num;
                float num4 = a.B * num;
                float num5 = a.A * num;
                float num6 = b.R * percentage;
                float num7 = b.G * percentage;
                float num8 = b.B * percentage;
                float num9 = b.A * percentage;
                return new Color(num2 + num6, num3 + num7, num4 + num8, num5 + num9);
            }

            /// <summary>
            /// Returns the average of all colors inside a collection.
            /// </summary>
            /// <param name="colors"></param>
            /// <returns></returns>
            public static Color MiddleMan(ICollection<Color> colors)
            {
                float num = 0f;
                float num2 = 0f;
                float num3 = 0f;
                float num4 = 0f;
                foreach (Color color in colors)
                {
                    num += color.R;
                    num2 += color.G;
                    num3 += color.B;
                    num4 += color.A;
                }
                num /= (float)colors.Count;
                num2 /= (float)colors.Count;
                num3 /= (float)colors.Count;
                num4 /= (float)colors.Count;
                return new Color(num, num2, num3, num4);
            }
        }

        /// <summary>
        /// Contains some utility methods concerting enumeration and generic collections.
        /// </summary>
        public static class Enumeration
        {
            /// <summary>
            /// Returns the amount of times an item is present in a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="item"></param>
            /// <returns></returns>
            public static int GetItemCount<T>(IEnumerable<T> collection, T item)
            {
                int count = 0;
                foreach(T colItem in collection)
                {
                    if (colItem.Equals(item))
                        count++;
                }
                return count;
            }

            /// <summary>
            /// Returns the amount of times a condition matched.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static int GetMatchCount<T>(IEnumerable<T> collection, Predicate<T> match)
            {
                int count = 0;
                foreach (T colItem in collection)
                {
                    if (match(colItem))
                        count++;
                }
                return count;
            }

            /// <summary>
            /// Returns the first empty index of a list; Returns -1 if none were found.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static int GetEmptyIndex<T>(IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i] == null)
                        return i;

                return -1;
            }
            
            /// <summary>
            /// Returns the first empty index of an array; Returns -1 if none were found.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <returns></returns>
            public static int GetEmptyIndex<T>(T[] array)
            {
                for (int i = 0; i < array.Length; i++)
                    if (array[i] == null)
                        return i;

                return -1;
            }

            /// <summary>
            /// Returns true if a list contains an item of type. (Only works on inherited classes)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static bool ListContainsType<T>(List<T> list, Type type)
            {
                foreach (T item in list)
                {
                    if (item.GetType() == type)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns an array which is a merged version of array1 and array2.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array1"></param>
            /// <param name="array2"></param>
            /// <returns></returns>
            public static T[] ArrayMerger<T>(T[] array1, T[] array2)
            {
                T[] array3 = new T[array1.Length + array2.Length];
                Array.Copy(array1, array3, array1.Length);
                Array.Copy(array2, 0, array3, array1.Length, array2.Length);
                return array3;
            }

            /// <summary>
            /// Equivalent to <paramref name="collection"/>.Intersect(<paramref name="objectsToFind"/>).Any().
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="objectsToFind"></param>
            /// <returns></returns>
            public static bool EnumerableContains<T>(IEnumerable<T> collection, IEnumerable<T> objectsToFind)
            {
                return collection.Intersect(objectsToFind).Any();
            }

            /// <summary>
            /// Returns true if the Array or all of it's elements are null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static bool IsEmpty<T>(IEnumerable<T> collection)
            {
                if (collection == null || collection.Count() < 1)
                {
                    return true;
                }
                foreach (T item in collection)
                {
                    if (item != null)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Equivalent to <see cref="List{T}.Find(Predicate{T})"/> for Arrays.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="toUse"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static T Find<T>(T[] toUse, Predicate<T> match)
                => Array.Find(toUse, match);

            /// <summary>
            /// Equivalent to <see cref="List{T}.FindIndex(Predicate{T})"/> for Arrays.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="toUse"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static int FindIndex<T>(T[] toUse, Predicate<T> match)
                => Array.FindIndex(toUse, match);

            /// <summary>
            /// Equivalent to <see cref="List{T}.ForEach(Action{T})"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <param name="action"></param>
            public static void ForEach<T>(T[] array, Action<T> action)
                => Array.ForEach(array, action);

            /// <summary>
            /// Removes all null elements inside a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            public static IEnumerable<T> RemoveNull<T>(IEnumerable<T> list)
            {
                return list.Where(t => t != null);
            }

            /// <summary>
            /// Removes all default elements inside a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            public static IEnumerable<T> RemoveDefault<T>(IEnumerable<T> list)
            {
                throw new Exception("The WarWolfWorks library is netstandard2.0 which does not support default generic type checks.");
            }

            /// <summary>
            /// Returns a generic <see cref="List{T}"/> from a non-generic <see cref="IList"/>. In case of an incorrect cast, it will return null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="linqReturn">If false, it will return a new list instead of using System.Linq to generate a list.</param>
            /// <returns></returns>
            public static List<T> ToGenericList<T>(IList list, bool linqReturn = true)
            {
                try
                {
                    if (linqReturn)
                        return list.Cast<T>().ToList();
                    else return new List<T>(list.Cast<T>());
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Returns a generic <see cref="IEnumerable{T}"/> from a non-generic <see cref="IEnumerable"/>. In case of an incorrect cast, it will return null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="enumerable"></param>
            /// <returns></returns>
            public static IEnumerable<T> ToGeneric<T>(IEnumerable enumerable)
            {
                try
                {
                    return enumerable.Cast<T>();
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Returns a <see cref="IEnumerable{T}"/> of instantiated objects (copies).
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static List<T> InstantiateList<T>(IEnumerable<T> objects) where T : IInstantiatable<T>
            {
                List<T> toReturn = new(objects);
                for (int i = 0; i < toReturn.Count; i++)
                {
                    toReturn[i] = toReturn[i].GetCopy();
                    toReturn[i].PostInstantiate();
                }

                return toReturn;
            }

            /// <summary>
            /// Returns an array of instantiated objects (copies).
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static T[] InstantiateArray<T>(T[] objects) where T : IInstantiatable<T>
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i] = objects[i].GetCopy();
                    objects[i].PostInstantiate();
                }

                return objects;
            }
        }

        /// <summary>
        /// Don't.
        /// </summary>
        public static class Disappointment
        {
            /// <summary>
            /// Returns a metric value into Bald eagle per obese child.
            /// </summary>
            /// <param name="meters"></param>
            /// <returns></returns>
            public static float MeterToBaldEaglePerObeseChild(float meters)
            {
                //height is 130cm for an average child,
                //to reach obese BMI (30) a 130cm child would need to be 78kg;
                //an average waist of 152 cm (diameter of 48.38cm, aka 152/π)
                //of which volume is 483.8x 130y 483.8z = .3042 m³ (3.042 if x10)
                //
                //bald eagle's length is 86 (median of 70–102) and
                //a weight of 4.65kg (median of 3-6.3kg),
                //and because we're comparing eagles to obese children:
                //78 / 4.65 = 16.77
                //so diameter of an eagle would be 48.38 / 16.77 = 2.88cm 
                //(28.8cm if x10, otherwise it's a tiny eagle lol, reality can be whatever I want it to be).
                //Volume is 28.8x 86y 28.8z = 0.071m³
                //so, (0.071 + 3.042 / 2) / 3 = 0.578.
                return 0.578f * meters;
            }

            /// <summary>
            /// Encrypts a local string value with a completely random encryption key.
            /// </summary>
            /// <param name="original"></param>
            public static void EncryptWithRandomKey(ref string original)
            {
                int size = 60;
                char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size * 4];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                { crypto.GetBytes(data); }
                StringBuilder key = new StringBuilder(size);
                for(int i = 0; i < size; i++)
                {
                    uint rand = BitConverter.ToUInt32(data, i * 4);
                    long index = rand % chars.Length;

                    key.Append(chars[index]);
                }
                original = RijndaelEncryption.Encrypt(original, key.ToString());
            }

            /// <summary>
            /// Causes a stack overflow.
            /// </summary>
            /// <param name="cause">Only causes a stack overflow if true.</param>
            /// <returns></returns>
            public static bool CauseStackOverflow(bool cause) => cause ? CauseStackOverflow(cause) : cause;
            
            /// <summary>
            /// Gets a random Unity component type.
            /// </summary>
            /// <returns></returns>
            public static Type GetRandomComponent()
            {
                Assembly assembly = typeof(EntityComponent).GetTypeInfo().Assembly;
                string nameSpace = "UnityEngine";
                Type[] types = assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
                return types[Random.Range(0, types.Length)];
            }
        }

        /// <summary>
        /// Contains all Regex, string and StringBuilder utilities.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// A regex expression used to match a given string as an acceptable interface name. (Used for files, as .cs is also counted as true)
            /// </summary>
            public static readonly Regex Is_InterfaceFile_Name = new Regex("^I[A-Z].+$");

            /// <summary>
            /// A regex expression used to see if a given string is an acceptable interface name. (Used for full names only, .cs and other extensions is counted as invalid)
            /// </summary>
            public static readonly Regex Is_Interface_Name = new Regex("^I[A-Z][a-zA-Z]+$");

            /// <summary>
            /// Puts a string value into a rainbow color using Unity's Rich Text format.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="frequency"></param>
            /// <param name="colorsToUse"></param>
            /// <returns></returns>
            public static string ToRainbow(string original, int frequency, Color[] colorsToUse)
            {
                string text = "<color=#klrtgiv>";
                string str = "</color>";
                char[] array = original.ToCharArray();
                string[] array2 = new string[array.Length];
                for (int i = 0; i < array.Length; i += frequency)
                {
                    int num = Math.Clamp(i + frequency, 0, array.Length);
                    for (int j = i; j < num; j++)
                    {
                        array2[j] = text.Replace("klrtgiv", Colors.ColorToHex(colorsToUse[MathF.ClampRounded(i, 0, colorsToUse.Length - 1)])) + array[j].ToString() + str;
                    }
                }
                string empty = string.Empty;
                empty = EnumerableConcat(empty, array2.ToList());
                return empty;
            }

            /// <summary>
            /// Puts a string value into a rainbow color using Unity's Rich Text format.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="frequency"></param>
            /// <param name="colorsToUse"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static string ToRainbow(string original, int frequency, Color[] colorsToUse, int offset)
            {
                string text = "<color=#klrtgiv>";
                string str = "</color>";
                char[] array = original.ToCharArray();
                string[] array2 = new string[array.Length];
                for (int i = 0; i < array.Length; i += frequency)
                {
                    int num = Math.Clamp(i + frequency, 0, array.Length);
                    for (int j = i; j < num; j++)
                    {
                        array2[j] = text.Replace("klrtgiv", Colors.ColorToHex(colorsToUse[MathF.ClampRounded(i + offset, 0, colorsToUse.Length - 1)])) + array[j].ToString() + str;
                    }
                }
                string empty = string.Empty;
                empty = EnumerableConcat(empty, array2.ToList());
                return empty;
            }

            /// <summary>
            /// Wraps original between wrappers.Item1 and wrappers.Item2
            /// </summary>
            /// <param name="original"></param>
            /// <param name="wrappers"></param>
            /// <returns></returns>
            public static string StringWrapper(string original, (char, char) wrappers)
            {
                StringBuilder stringBuilder = new StringBuilder(3);
                stringBuilder.Insert(0, wrappers.Item1);
                stringBuilder.Insert(1, original);
                stringBuilder.Insert(original.Length + 1, wrappers.Item2);
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Wraps original between startWrapper and endWrapper.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="startWrapper"></param>
            /// <param name="endWrapper"></param>
            /// <returns></returns>
            public static string StringWrapper(string original, char startWrapper, char endWrapper)
            {
                StringBuilder stringBuilder = new StringBuilder(3);
                stringBuilder.Insert(0, startWrapper);
                stringBuilder.Insert(1, original);
                stringBuilder.Insert(original.Length + 1, endWrapper);
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Cuts out a string between from and to.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static string Cutout(string original, string from, string to)
            {
                int pFrom = original.IndexOf(from) + from.Length;
                int pTo = original.LastIndexOf(to);
                return original.Substring(pFrom, pTo - pFrom);
            }

            private static readonly Regex kanjiRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            private static readonly Regex hiraganaRegex = new Regex(@"\p{IsHiragana}");
            private static readonly Regex katakanaRegex = new Regex(@"\p{IsKatakana}");

            /// <summary>
            /// Returns true if the given string contains a Kanji character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKanji(string value)
                => kanjiRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Hiragana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsHiragana(string value)
                => hiraganaRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKatakana(string value)
               => katakanaRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains either a Kanji, Hiragana or Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsJapanese(string value)
                => IsKanji(value) || IsHiragana(value) || IsKatakana(value);

            /// <summary>
            /// Cuts out a string between from and to in char values.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static string Cutout(string original, char from, char to)
            {
                int pFrom = original.IndexOf(from) + 1;
                int pTo = original.LastIndexOf(to);

                return original.Substring(pFrom, pTo - pFrom);
            }
        }

        /// <summary>
        /// Contains all mathematical methods, fields and properties.
        /// </summary>
        public static class MathF
        {
            #region Constants
            /// <summary>
            /// The value for which all absolute numbers smaller than are considered equal to zero.
            /// </summary>
            public const float Epsilon = 1e-6f;

            /// <summary>
            /// The infamous value of the circle-people.
            /// </summary>
            public const float PI = (float)Math.PI;

            /// <summary>
            /// Equivalent to float.PositiveInfinity.
            /// </summary>
            public const float Infinity = float.PositiveInfinity;

            /// <summary>
            /// Equivalent to float.NegativeInfinity.
            /// </summary>
            public const float NegativeInfinity = float.NegativeInfinity;

            /// <summary>
            /// Degrees-to-radians conversion constant.
            /// </summary>
            public const float Deg2Rad = 0.0174532924f;

            /// <summary>
            /// Radians-to-degrees conversion constant.
            /// </summary>
            public const float Rad2Deg = 57.29578f;
            #endregion

            #region Misc
            /// <summary>
            /// Returns a value with the magnitude of x and the sign of y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static double CopySign(double x, double y)
            {
                return y >= 0d ? x : -x;
            }

            /// <summary>
            /// Returns a value with the magnitude of x and the sign of y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static float CopySign(float x, float y)
            {
                return y >= 0f ? x : -x;
            }

            /// <summary>
            ///   <para>Moves the value current towards target.</para>
            /// </summary>
            /// <param name="current">The current value.</param>
            /// <param name="target">The value to move towards.</param>
            /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
            public static float MoveTowards(float current, float target, float maxDelta)
            {
                if (!(Math.Abs(target - current) <= maxDelta))
                {
                    return current + System.MathF.Sign(target - current) * maxDelta;
                }
                return target;
            }

            /// <summary>
            /// Returns: value / maxValue * (maxValue - value)
            /// </summary>
            /// <param name="value"></param>
            /// <param name="maxValue"></param>
            /// <returns></returns>
            public static float DeminishingReturnHalfCurve(float value, float maxValue)
            {
                return value / maxValue * (maxValue - value);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <returns></returns>
            public static float Hyperbolic(float max, float amount)
            {
                return max - max / (max + amount);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <returns></returns>
            public static double Hyperbolic(double max, double amount)
            {
                return max - max / (max + amount);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically, to the power of P.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <param name="P"></param>
            /// <returns></returns>
            public static float HyperbolicP(float max, float amount, float P)
            {
                return System.MathF.Pow(max - max / (max + amount), P);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically, to the power of P.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <param name="P"></param>
            /// <returns></returns>
            public static double HyperbolicP(double max, double amount, double P)
            {
                return Math.Pow(max - max / (max + amount), P);
            }

            /// <summary>
            /// Clamps a double value between min and max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static double Clamp(double value, double min, double max)
            {
                double aMin = Math.Min(min, max);
                double aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }

            /// <summary>
            /// Clamps a float value between min and max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static float Clamp(float value, float min, float max)
            {
                float aMin = Math.Min(min, max);
                float aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }

            /// <summary>
            /// Clamps an int value between min and max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int Clamp(int value, int min, int max)
            {
                int aMin = Math.Min(min, max);
                int aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }

            /// <summary>
            /// Attempts to clamp an euler rotation value.
            /// </summary>
            /// <param name="angle"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static float ClampAngle(float angle, float from, float to)
            {
                if (angle < 0f)
                {
                    angle = 360f + angle;
                }
                if (angle > 180f)
                {
                    return Math.Max(angle, 360f + from);
                }
                return Math.Min(angle, to);
            }

            /// <summary>
            /// Attempts to clamp a euler rotation value.
            /// </summary>
            /// <param name="angle"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampAngle(float angle, FloatRange range)
            {
                if (angle < 0f)
                {
                    angle = 360f + angle;
                }
                if (angle > 180f)
                {
                    return Math.Max(angle, 360f + range.Min);
                }
                return Math.Min(angle, range.Max);
            }

            /// <summary>
            /// Cuts a float to digits length.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="digits"></param>
            /// <returns></returns>
            public static float Truncate(float value, int digits)
            {
                double num = Math.Pow(10.0, digits);
                double num2 = Math.Truncate(num * value) / num;
                return (float)num2;
            }

            /// <summary>
            /// Rounds a float value.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="favorLower"></param>
            /// <returns></returns>
            public static int RoundValue(float value, bool favorLower)
            {
                float num = value - (float)(int)value;
                if ((num <= 0.5f && favorLower) || (num < 0.5f && !favorLower))
                {
                    return (int)(value - num);
                }
                return (int)(1f - num + value);
            }

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToPositive(int value)
                => Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToNegative(int value)
            => -System.Math.Abs(value);

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToPositive(float value)
            => System.Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToNegative(float value)
            => -System.Math.Abs(value);

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToPositive(long value)
            => System.Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToNegative(long value)
            => -System.Math.Abs(value);

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float Clamp(float value, FloatRange range)
            => Clamp(value, range.Min, range.Max);

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int Clamp(int value, IntRange range)
                => Clamp(value, range.Min, range.Max);

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static float ClampRounded(float value, float min, float max)
            {
                float num = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampRounded if the minimal value is equal to or higher than the maximal value.");
                }
                while (num > max)
                {
                    num = max == 0 ? min : num - max;
                }
                while (num < min)
                {
                    num = min == 0 ? max : num + min;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampRounded(float value, FloatRange range) => ClampRounded(value, range.Min, range.Max);

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int ClampRounded(int value, int min, int max)
            {
                int i = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampRounded if the minimal value is equal to or higher than the maximal value.");
                }
                while (i > max || i < min)
                {
                    if (i > max) i = max == 0 ? min : i - max;
                    else
                    {
                        if (min == 0)
                        {
                            i = 0;
                            break;
                        }
                        else
                            i += min;
                    }
                }
                return i;
            }

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int ClampRounded(int value, IntRange range) => ClampRounded(value, range.Min, range.Max);

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int ClampPingPong(int value, int min, int max)
            {
                int num = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > max)
                    {
                        int num2 = num - max;
                        num -= num2;
                        continue;
                    }
                    if (num < min)
                    {
                        int num3 = min - num;
                        num += num3;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int ClampPingPong(int value, IntRange range) => ClampPingPong(value, range.Min, range.Max);

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static float ClampPingPong(float value, float min, float max)
            {
                float num = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > max)
                    {
                        float num2 = num - max;
                        num -= num2 * 2f;
                        continue;
                    }
                    if (num < min)
                    {
                        float num3 = min - num;
                        num += num3 * 2f;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampPingPong(float value, FloatRange range) => ClampPingPong(value, range.Min, range.Max);

            /// <summary>
            /// Returns true if a float is NaN, Infinity or Negative Infinity.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsFormal(float value)
            {
                return float.IsNaN(value) || float.IsInfinity(value) || float.IsNegativeInfinity(value);
            }

            /// <summary>
            /// Returns true if numberToApproximate is not lower than or higer than numberToCompare using approximation as range.
            /// </summary>
            /// <param name="NumberToApproximate"></param>
            /// <param name="NumberToCompare"></param>
            /// <param name="approximation"></param>
            /// <returns></returns>
            public static bool IsApproximate(float NumberToApproximate, float NumberToCompare, float approximation)
            {
                bool result = false;
                float num = (NumberToApproximate - NumberToCompare).ToPositive();
                if (num < approximation)
                {
                    result = true;
                }
                return result;
            }

            /// <summary>
            /// Returns a mid-point between two values.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static float MidPoint(float a, float b, float percent = 0.5f)
            {
                return (b + a) * percent;
            }
            
            /// <summary>
            /// Returns a mid-point between two values.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static double MidPoint(double a, double b, double percent = 0.5f)
            {
                return (b + a) * percent;
            }

            /// <summary>
            /// Returns true if the value given is not higher than range.Max nor lower than range.Min.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static bool IsWithinRange(float value, FloatRange range)
                => value < range.Max && value > range.Min;

            /// <summary>
            /// Returns true if the value given is not higher than range.Max nor lower than range.Min.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static bool IsWithinRange(int value, IntRange range)
                => value < range.Max && value > range.Min;

            #endregion  

            #region Quaternions & Rotations
            /// <summary>
            /// Returns a quaternion from a euler rotation.
            /// </summary>
            /// <param name="euler">The rotation in Vector3 space.</param>
            /// <returns></returns>
            public static Quaternion Euler(Vector3 euler)
            {
                YPRRotation(euler.X, euler.Y, euler.Z, out Quaternion toReturn);
                return toReturn;
            }

            /// <summary>
            /// Returns a quaternion based on yaw, pitch and roll.
            /// </summary>
            /// <param name="yaw">The yaw, equivalent to the X axis in euler.</param>
            /// <param name="pitch">The pitch, equivalent to the Y axis in euler.</param>
            /// <param name="roll">The roll, equivalent to the Z axis in euler.</param>
            /// <param name="result">Returned quaternion.</param>
            public static void YPRRotation(float yaw, float pitch, float roll, out Quaternion result)
            {
                //Abbreviated for the sake of readability
                float hr = roll * 0.5f;
                float hp = pitch * 0.5f;
                float hy = yaw * 0.5f;

                float sr = System.MathF.Sin(hr);
                float cr = System.MathF.Cos(hr);
                float sp = System.MathF.Sin(hp);
                float cp = System.MathF.Cos(hp);
                float sy = System.MathF.Sin(hy);
                float cy = System.MathF.Cos(hy);

                result.X = cy * sp * cr + sy * cp * sr;
                result.Y = sy * cp * cr - cy * sp * sr;
                result.Z = cy * cp * sr - sy * sp * cr;
                result.W = cy * cp * cr + sy * sp * sr;
            }

            /// <summary>
            /// Returns the angle in degrees between two rotations a and b.
            /// </summary>
            public static float Angle(Quaternion a, Quaternion b)
            {
                float f = Quaternion.Dot(a, b);
                
                return System.MathF.Acos(System.MathF.Min(System.MathF.Abs(f), 1f)) * 2f * Rad2Deg;
            }

            /// <summary>
            ///   <para>Rotates a rotation from towards to.</para>
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="maxDegreesDelta"></param>
            public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
            {
                float num = Angle(from, to);
                if (num != 0f)
                {
                    float t = System.MathF.Min(1f, maxDegreesDelta / num);
                    return Quaternion.Slerp(from, to, t);
                }
                return to;
            }

            /// <summary>
            /// Equivalent to <see cref="RotateTowards(Quaternion, Quaternion, float)"/>.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="speed"></param>
            /// <returns></returns>
            public static Vector3 EulerRotateTowards(Vector3 from, Vector3 to, float speed)
            {
                return RotateTowards(Euler(from), Euler(to), speed).EulerAngles();
            }

            public static Vector3 Multiply(Quaternion rotation, Vector3 point)
            {
                float num = rotation.X * 2f;
                float num2 = rotation.Y * 2f;
                float num3 = rotation.Z * 2f;
                float num4 = rotation.X * num;
                float num5 = rotation.Y * num2;
                float num6 = rotation.Z * num3;
                float num7 = rotation.X * num2;
                float num8 = rotation.X * num3;
                float num9 = rotation.Y * num3;
                float num10 = rotation.W * num;
                float num11 = rotation.W * num2;
                float num12 = rotation.W * num3;
                Vector3 result = new Vector3(
                (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z,
                (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num9 - num10) * point.Z,
                (num8 - num11) * point.X + (num9 + num10) * point.Y + (1f - (num4 + num5)) * point.Z);
                return result;
            }

            #endregion

            #region Vectors

#pragma warning disable 1591
            public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);
            public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);
            public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);
            public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);
            public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);
            public static readonly Vector3 Back = new Vector3(0f, 0f, -1f);
            public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
            public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);

            public static readonly Vector2 Up2 = new Vector2(0f, 1f);
            public static readonly Vector2 Down2 = new Vector2(0f, -1f);
            public static readonly Vector2 Left2 = new Vector2(-1f, 0f);
            public static readonly Vector2 Right2 = new Vector2(1f, 0f);
            public static readonly Vector2 One2 = new Vector2(1f, 1f);
            public static readonly Vector2 Zero2 = new Vector2(0f, 0f);
#pragma warning restore 1591
            /// <summary>
            /// Converts a quaternion into euler angles.
            /// </summary>
            /// <param name="q">The rotation in quaternion.</param>
            public static Vector3 EulerAngles(Quaternion q)
            {
                // roll (x-axis rotation)
                double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
                double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
                double roll = Math.Atan2(sinr_cosp, cosr_cosp);

                // pitch (y-axis rotation)
                double sinp = 2 * (q.W * q.Y - q.Z * q.X);
                double pitch;
                if (System.Math.Abs(sinp) >= 1)
                    pitch = CopySign(PI / 2, sinp); // use 90 degrees if out of range
                else
                    pitch = Math.Asin(sinp);

                // yaw (z-axis rotation)
                double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
                double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
                double yaw = Math.Atan2(siny_cosp, cosy_cosp);

                return new Vector3((float)roll, (float)pitch, (float)yaw);
            }

            /// <summary>
            ///   <para>Moves a point current towards target.</para>
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
            {
                Vector2 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            /// Moves a point current towards target.
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
            {
                Vector3 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            ///   <para>Moves a point current towards target.</para>
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
            {
                Vector4 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector2 original)
            {
                return System.MathF.Sqrt(Vector2.Dot(original, original));
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector3 original)
            {
                return System.MathF.Sqrt(Vector3.Dot(original, original));
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector4 original)
            {
                return System.MathF.Sqrt(Vector4.Dot(original, original));
            }

            /// <summary>
            /// Returns direction which would make Position look at Destination.
            /// </summary>
            /// <param name="Position"></param>
            /// <param name="Destination"></param>
            /// <returns></returns>
            public static Vector3 DirectionTowards(Vector3 Position, Vector3 Destination)
            {
                Vector3 toReturn = (Destination - Position);
                toReturn.Normalize();
                return toReturn;
            }

            /// <summary>
            /// Returns the total of vectors divided by Length of array given.
            /// </summary>
            /// <param name="values"></param>
            /// <returns></returns>
            public static Vector3 Average(Vector3[] values)
            {
                Vector3 toReturn = Vector3.Zero;

                values.ForEach(v => toReturn += v);

                toReturn /= values.Length;

                return toReturn;
            }

            /// <summary>
            /// Returns in order: Back, Down, Forward, Left, Right, Up.
            /// </summary>
            public static Vector3[] AllDirections => new Vector3[]
            {
                Back,
                Down,
                Forward,
                Left,
                Right,
                Up,
            };
            /// <summary>
            /// Returns in order:
            /// Right2, One2, Up2, new Vector2(-1f, 1f), Left2, new Vector2(-1f, -1f), Down2, new Vector2(1f, -1f).
            /// </summary>
            public static Vector2[] AllDirections2D => new Vector2[8]
            {
                Right2,
                One2,
                Up2,
                new Vector2(-1f, 1f),
                Left2,
                new Vector2(-1f, -1f),
                Down2,
                new Vector2(1f, -1f),
            };

            /// <summary>
            /// Returns in order: One2, new Vector2(-1f, 1f), -One2, new Vector2(1f, -1f).
            /// </summary>
            public static Vector2[] DiagonalDirections2D => new Vector2[4]
            {
                One2,
                new Vector2(-1f, 1f),
                -One2,
                new Vector2(1f, -1f)
            };

            /// <summary>
            /// Returns in order: Right2, Up2, Left2, Down2.
            /// </summary>
            public static Vector2[] MainDirections2D => new Vector2[4]
            {
                Right2,
                Up2,
                Left2,
                Down2
            };

            /// <summary>
            /// Returns in order: Left2, Right2.
            /// </summary>
            public static Vector2[] HorizontalSides2D => new Vector2[2]
            {
                Left2,
                Right2,
            };

            /// <summary>
            /// Returns in order: Up2, Down2.
            /// </summary>
            public static Vector2[] VerticalSides2D => new Vector2[2]
            {
                Up2,
                Down2
            };

            /// <summary>
            /// Rotates point around pivot based on angle (Euler).
            /// </summary>
            /// <param name="point"></param>
            /// <param name="pivot"></param>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
            {
                return Multiply(Euler(angle), (point - pivot)) + pivot;
            }

            /// <summary>
            /// Rotates point around pivot based on angle (Quaternion).
            /// </summary>
            /// <param name="point"></param>
            /// <param name="pivot"></param>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
            {
                return Multiply(angle, (point - pivot)) + pivot;
            }

            /// <summary>
            /// Returns a Vector2 that has the highest axis value kept, and the lowest set to 0.
            /// </summary>
            /// <param name="evaluator"></param>
            /// <returns></returns>
            public static Vector2 HighestValue(Vector2 evaluator)
            {
                float num = evaluator.X;
                float num2 = evaluator.Y;
                bool flag = false;
                bool flag2 = false;
                if (num < 0f)
                {
                    flag = true;
                    num = 0f - num;
                }
                if (num2 < 0f)
                {
                    flag2 = true;
                    num2 = 0f - num2;
                }
                if (num == 0f && flag2)
                {
                    flag = true;
                }
                if (num2 == 0f && flag)
                {
                    flag2 = true;
                }
                Vector2 result = new Vector2((num > num2) ? ((!flag) ? 1 : (-1)) : 0, (num2 > num) ? ((!flag2) ? 1 : (-1)) : 0);
                return result;
            }

            /// <summary>
            /// Snaps a vector to given factor. Useful to make grid-like behaviour.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="factor"></param>
            /// <returns></returns>
            public static Vector3 Snap(Vector3 vector, float factor)
            {
                factor = factor.ToPositive();
                float x = (float)System.Math.Round(vector.X / factor) * factor;
                float y = (float)System.Math.Round(vector.Y / factor) * factor;
                float z = (float)System.Math.Round(vector.Z / factor) * factor;
                return new Vector3(x, y, z);
            }

            /// <summary>
            /// Snaps a vector to given factor, while ignoring all corresponding Ignore values.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="factor"></param>
            /// <param name="IgnoreX"></param>
            /// <param name="IgnoreY"></param>
            /// <param name="IgnoreZ"></param>
            /// <returns></returns>
            public static Vector3 Snap(Vector3 vector, float factor, bool IgnoreX, bool IgnoreY, bool IgnoreZ)
            {
                factor = factor.ToPositive();
                float x = IgnoreX ? vector.X : (System.MathF.Round(vector.X / factor) * factor);
                float y = IgnoreY ? vector.Y : (System.MathF.Round(vector.Y / factor) * factor);
                float z = IgnoreZ ? vector.Z : (System.MathF.Round(vector.Z / factor) * factor);
                return new Vector3(x, y, z);
            }

            /// <summary>
            /// Returns a Vector2 that has the lowest axis value kept, and the highest set to 0.
            /// </summary>
            /// <param name="evaluator"></param>
            /// <returns></returns>
            public static Vector2 LowestValue(Vector2 evaluator)
            {
                float num = evaluator.X;
                float num2 = evaluator.Y;
                bool flag = false;
                bool flag2 = false;
                if (num < 0f)
                {
                    flag = true;
                    num = 0f - num;
                }
                if (num2 < 0f)
                {
                    flag2 = true;
                    num2 = 0f - num2;
                }
                if (num == 0f && flag2)
                {
                    flag = true;
                }
                if (num2 == 0f && flag)
                {
                    flag2 = true;
                }
                Vector2 result = new Vector2((num < num2) ? ((!flag) ? 1 : (-1)) : 0, (num2 < num) ? ((!flag2) ? 1 : (-1)) : 0);
                return result;
            }

            /// <summary>
            /// Equivalent to <see cref="Vector3.Distance(Vector3, Vector3)"/>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float DistanceBetween(Entity a, Entity b)
            {
                float result = float.PositiveInfinity;
                if (a != null && b != null)
                {
                    result = Vector3.Distance(a.Transform.Position, b.Transform.Position);
                }
                return result;
            }

            /// <summary>
            /// Equivalent to <see cref="Vector2.Distance(Vector2, Vector2)"/>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float DistanceBetween(Vector2 a, Vector2 b)
            {
                return Vector2.Distance(a, b);
            }

            /// <summary>
            /// Returns the distance of an axis between two Vector2 values.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="axisChoice"></param>
            /// <returns></returns>
            public static float DistanceBetween(Vector2 a, Vector2 b, Axis axisChoice)
            {
                float num;
                float num2;
                if (axisChoice == Axis.X)
                {
                    num = a.X;
                    num2 = b.X;
                }
                else
                {
                    num = a.X;
                    num2 = b.Y;
                }
                float num3 = num - num2;
                if (num3 < 0f)
                {
                    num3 = 0f - num3;
                }
                return num3;
            }

            /// <summary>
            /// Returns true if position is wihin bounds. (bounds are: x and y as min, z and w as max (z is max X and w is max Y))
            /// </summary>
            /// <param name="position"></param>
            /// <param name="bounds"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector2 position, Vector4 bounds)
            {
                float x = position.X;
                float y = position.Y;
                return x > bounds.X && x < bounds.Z && y > bounds.Y && y < bounds.W;
            }

            /// <summary>
            /// Returns true if position is within size from center. 
            /// </summary>
            /// <param name="position"></param>
            /// <param name="center"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector3 position, Vector3 center, Vector3 size)
            {
                Vector3 compared = position - center;

                return compared.X > -size.X && compared.X < size.X
                    && compared.Y > -size.Y && compared.Y < size.Y
                    && compared.Z > -size.Z && compared.Z < size.Z;
            }

            /// <summary>
            /// Returns true if the position is within the radius of center.
            /// </summary>
            /// <param name="position"></param>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector3 position, Vector3 center, float radius)
                => Vector3.Distance(position, center) < radius;

            /// <summary>
            /// Returns true if position is wihin min and max.
            /// </summary>
            /// <param name="position"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector2 position, Vector2 min, Vector2 max)
                => IsInsideBounds(position, new Vector4(min.X, min.Y, max.X, max.Y));

            /// <summary>
            /// Returns a Vector2 which has it's Y value set to the given Vector3 Z value.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector2 Vector3D(Vector3 vector)
            {
                return new Vector2(vector.X, vector.Z);
            }

            /// <summary>
            /// Returns a Vector3 which moves a towards b based on speed. Accelerates based on the distance between both positions.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="speed"></param>
            /// <param name="acceleration"></param>
            /// <returns></returns>
            public static Vector3 MoveTowardsAccelerated(Vector3 a, Vector3 b, float speed, float acceleration)
            {
                float num = acceleration * Vector3.Distance(a, b);
                float maxDistanceDelta = speed + num;
                return MathF.MoveTowards(a, b, maxDistanceDelta);
            }

            /// <summary>
            /// Returns a Vector3 which moves a towards b based on speed. Accelerates based on the distance between both positions.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="speed"></param>
            /// <param name="acceleration"></param>
            /// <returns></returns>
            public static Vector3 MoveTowardsAccelerated(TransformComponent a, TransformComponent b, float speed, float acceleration)
            {
                return MoveTowardsAccelerated(a.Position, b.Position, speed, acceleration);
            }

            /// <summary>
            /// Equivalent to <see cref="MathF.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="minX"></param>
            /// <param name="maxX"></param>
            /// <param name="minY"></param>
            /// <param name="maxY"></param>
            /// <returns></returns>
            public static Vector2 Clamp(Vector2 toClamp, float minX, float maxX, float minY, float maxY)
            {
                return new Vector2(Math.Clamp(toClamp.X, minX, maxX), Math.Clamp(toClamp.Y, minY, maxY));
            }

            /// <summary>
            /// Equivalent to <see cref="MathF.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="minX"></param>
            /// <param name="maxX"></param>
            /// <param name="minY"></param>
            /// <param name="maxY"></param>
            /// <param name="minZ"></param>
            /// <param name="maxZ"></param>
            /// <returns></returns>
            public static Vector3 Clamp(Vector3 toClamp, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
            {
                return new Vector3(Math.Clamp(toClamp.X, minX, maxX), Math.Clamp(toClamp.Y, minY, maxY), Math.Clamp(toClamp.Z, minZ, maxZ));
            }

            /// <summary>
            /// Equivalent to <see cref="MathF.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector3 Clamp(Vector3 toClamp, Vector3 min, Vector3 max)
            {
                return new Vector3(Math.Clamp(toClamp.X, min.X, max.X), Math.Clamp(toClamp.Y, min.Y, max.Y), Math.Clamp(toClamp.Z, min.Z, max.Z));
            }

            /// <summary>
            /// Sets NaN, null or Infinite values of the given Vector to 0.
            /// </summary>
            /// <param name="toReform"></param>
            /// <returns></returns>
            public static Vector3 Reformalize(Vector3 toReform)
            {
                Vector3 vector = new Vector3(MathF.IsFormal(toReform.X) ? 0f : toReform.X, MathF.IsFormal(toReform.Y) ? 0f : toReform.Y, MathF.IsFormal(toReform.Z) ? 0f : toReform.Z);
                return vector;
            }

            /// <summary>
            /// Returns true if PositionToApproximate is within approximation of PositionToCompare.
            /// </summary>
            /// <param name="PositionToApproximate"></param>
            /// <param name="PositionToCompare"></param>
            /// <param name="approximation"></param>
            /// <returns></returns>
            public static bool IsApproximate(Vector3 PositionToApproximate, Vector3 PositionToCompare, Vector3 approximation)
            {
                bool flag = PositionToApproximate.X.IsApproximate(PositionToCompare.X, approximation.X);
                bool flag2 = PositionToApproximate.Y.IsApproximate(PositionToCompare.Y, approximation.Y);
                bool flag3 = PositionToApproximate.Z.IsApproximate(PositionToCompare.Z, approximation.Z);
                return (flag && flag2) & flag3;
            }

            /// <summary>
            /// Returns a position between a and b, based on percent (0-1).
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static Vector2 MiddleMan(Vector2 a, Vector2 b, float percent = 0.5f)
            {
                float num = MathF.MidPoint(a.X, b.X, percent);
                float num2 = MathF.MidPoint(a.Y, b.Y, percent);
                return new Vector2(a.X + num, a.Y + num2);
            }

            /// <summary>
            /// Gets the closest element to the given point from a given collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static T GetClosestToPoint<T>(Vector3 point, float maxDistance, IEnumerable<T> collection) where T : EntityComponent
            {
                T result = default(T);
                float num = float.PositiveInfinity;
                foreach (T item in collection)
                {
                    float num2 = Vector3.Distance(item.Entity.Transform.Position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = item;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets the closest gameobject to the given point within a maximum distance.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <returns></returns>
            public static Entity GetClosestToPoint(EntityManager manager, Vector3 point, float maxDistance)
            {
                Entity result = null;
                float num = float.PositiveInfinity;
                foreach (Entity entity in manager)
                {
                    float num2 = Vector3.Distance(entity.Transform.Position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = entity;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets the closest gameobject to the given point within a maximum distance.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <param name="excluded">Any gameobject in this list will be skipped.</param>
            /// <returns></returns>
            public static Entity GetClosestToPoint(EntityManager manager, Vector3 point, float maxDistance, Entity[] excluded)
            {
                Entity result = null;
                float num = float.PositiveInfinity;
                foreach (Entity gameObject in manager)
                {
                    if (Array.IndexOf(excluded, gameObject) != -1)
                        continue;

                    float num2 = Vector3.Distance(gameObject.Transform.Position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = gameObject;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets all entities within a radius.
            /// </summary>
            public static List<Entity> GetAllWithinDistance(EntityManager manager, Vector3 center, float radius)
            {
                List<Entity> list = new List<Entity>();
                foreach(Entity entity in manager)
                {
                    if (Vector3.Distance(center, entity.Transform.Position) < radius)
                        list.Add(entity);
                }

                return list;
            }

            /// <summary>
            /// Gets all entities within a radius and an additional condition.
            /// </summary>
            public static List<Entity> GetAllWithinDistance(EntityManager manager, Vector3 center, float radius, Predicate<Entity> condition)
            {
                List<Entity> list = new List<Entity>();
                foreach (Entity entity in manager)
                {
                    if (Vector3.Distance(center, entity.Transform.Position) < radius && condition(entity))
                        list.Add(entity);
                }

                return list;
            }

            /// <summary>
            /// Returns x + y + z / 3 if includeZValue is true, otherwise returns x + y / 2.
            /// </summary>
            /// <param name="vector3"></param>
            /// <param name="includeZValue"></param>
            /// <returns></returns>
            public static float AverageValue(Vector3 vector3, bool includeZValue)
            {
                return (vector3.X + vector3.Y + (includeZValue ? vector3.Z : 0f)) / (float)(includeZValue ? 3 : 2);
            }

            /// <summary>
            /// Gets all entities who's Positions are closest to the center, based on amount requested.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="from"></param>
            /// <param name="amount"></param>
            /// <exception cref="ArgumentOutOfRangeException"/>
            /// <returns></returns>
            public static Entity[] GetClosestToPosition(Vector3 center, EntityManager from, int amount)
            {
                int count = from.Count;

                if (amount > count)
                    throw new ArgumentOutOfRangeException("Cannot use GetClosestToPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    Debug.LogWarning("Using GetClosestToPosition with amount equal to the collection size is pointless. Returning original collection...");
                    return from.ToArray();
                }

                Entity[] toReturn = new Entity[amount];

                foreach (Entity entity in from)
                {
                    int index = Array.FindIndex(toReturn, i => i == null || Vector3.Distance(center, i.Transform.Position) > Vector3.Distance(center, entity.Transform.Position));
                    if (index == -1)
                        continue;

                    toReturn[index] = entity;
                }
                return toReturn;
            }

            /// <summary>
            /// Return all components who's .transform.position is the furthest from center.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="from"></param>
            /// <param name="amount"></param>
            /// <returns></returns>
            public static Entity[] GetFurthestFromPosition<T>(Vector3 center, EntityManager from, int amount)
            {
                int count = from.Count();
                if (amount > count)
                    throw new Exception("Cannot use GetFurthestFromPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    Debug.LogWarning("Using GetFurthestFromPosition with amount equal to the collection size is pointless. Returning original collection...");
                    return from.ToArray();
                }

                Entity[] toReturn = new Entity[amount];

                foreach (Entity entity in from)
                {
                    int index = Array.FindIndex(toReturn, i => i == null || Vector3.Distance(center, i.Transform.Position) < Vector3.Distance(center, entity.Transform.Position));
                    if (index == -1)
                        continue;

                    toReturn[index] = entity;
                }
                return toReturn;
            }

            /// <summary>
            /// Gets all components who's .transform.position is within center by radius.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <param name="used"></param>
            /// <returns></returns>
            public static T[] GetAllWithinDistance<T>(Vector3 center, float radius, IEnumerable<T> used) where T : EntityComponent
            {
                List<T> toReturn = new List<T>(used);
                toReturn.RemoveAll(t => Vector3.Distance(center, t.Entity.Transform.Position) > radius);

                return toReturn.ToArray();
            }

            /// <summary>
            /// Gets all components who's .transform.position is within center by range.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="range"></param>
            /// <param name="used"></param>
            /// <returns></returns>
            public static T[] GetAllWithinRange<T>(Vector3 center, FloatRange range, IEnumerable<T> used) where T : EntityComponent
            {
                List<T> toReturn = new List<T>(used);
                toReturn.RemoveAll(
                    t =>
                    {
                        float Distance = Vector3.Distance(center, t.Entity.Transform.Position);
                        return Distance > range.Max || Distance < range.Min;
                    }
                    );

                return toReturn.ToArray();
            }
            #endregion
        }

        /// <summary>
        /// Returns MethodInfo of all public methods inside <see cref="Hooks"/>.
        /// </summary>
        public static IEnumerable<MethodInfo> HooksMethods = typeof(Hooks).GetMethods();

        /// <summary>
        /// Returns <see cref="MethodInfo"/> of all public methods inside <see cref="Hooks"/> and all of it's nested classes.
        /// </summary>
        public static List<MethodInfo> FullMethods
        {
            get
            {
                List<MethodInfo> toReturn = new List<MethodInfo>(typeof(Hooks).GetMethods());

                Type[] types = typeof(Hooks).GetNestedTypes();
                for(int i = 0; i < types.Length; i++)
                {
                    toReturn.AddRange(types[i].GetMethods());
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Tries to parse a type with <see cref="Type.GetType(string)"/>; 
        /// if it fails, it then tries to itterate through every Assembly inside <see cref="AppDomain.CurrentDomain"/> to parse the given typeName.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type ParseType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        /// <summary>
        /// Returns the CompareTo method based on integer type of the value given; If the object(s) given are non-standard data (like int, float, double, etc) it will return 0.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="matchType">If true, both objects must be of the same type to work.</param>
        /// <returns></returns>
        public static int CompareTo(object a, object b, bool matchType)
        {
            try
            {
                if (matchType)
                {
                    switch (a)
                    {
                        default: return 0;
                        case byte Byte when b is byte:
                            return Byte.CompareTo((byte)b);
                        case sbyte sByte when b is sbyte:
                            return sByte.CompareTo((sbyte)b);
                        case short Short when b is short:
                            return Short.CompareTo((short)b);
                        case ushort uShort when b is ushort:
                            return uShort.CompareTo((ushort)b);
                        case int Int when b is int:
                            return Int.CompareTo((int)b);
                        case uint uInt when b is uint:
                            return uInt.CompareTo((uint)b);
                        case long Long when b is long:
                            return Long.CompareTo((long)b);
                        case ulong uLong when b is ulong:
                            return uLong.CompareTo((ulong)b);
                        case float Float when b is float:
                            return Float.CompareTo((float)b);
                        case double Double when b is double:
                            return Double.CompareTo((double)b);
                    }
                }
                else
                {
                    switch (a)
                    {
                        default: return 0;
                        case byte Byte when b is byte:
                            return Byte.CompareTo(b);
                        case sbyte sByte when b is sbyte:
                            return sByte.CompareTo(b);
                        case short Short when b is short:
                            return Short.CompareTo(b);
                        case ushort uShort when b is ushort:
                            return uShort.CompareTo(b);
                        case int Int when b is int:
                            return Int.CompareTo(b);
                        case uint uInt when b is uint:
                            return uInt.CompareTo(b);
                        case long Long when b is long:
                            return Long.CompareTo(b);
                        case ulong uLong when b is ulong:
                            return uLong.CompareTo(b);
                        case float Float when b is float:
                            return Float.CompareTo(b);
                        case double Double when b is double:
                            return Double.CompareTo(b);
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Itterates through each string, and uses <see cref="ParseType(string)"/>.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List<Type> ParseTypes(IEnumerable<string> types)
        {
            List<Type> toReturn = new List<Type>(types.Count());
            foreach (string s in types)
                toReturn.Add(ParseType(s));

            return toReturn;
        }

        /// <summary>
        /// Returns true if type is implementing implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public static bool Implements(Type type, Type implementation)
            => implementation.IsAssignableFrom(type);

        /// <summary>
        /// Returns true if type is implementing the generic implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericImplementation"></param>
        /// <returns></returns>
        public static bool ImplementsGeneric(Type type, Type genericImplementation)
            => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericImplementation);

        private const uint CF_TEXT = 1u;

        private const uint CF_UNICODETEXT = 13u;

        /// <summary>
        /// Removes element at toRemove index from ref list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="toRemove"></param>
        public static void RemoveElement(ref List<int> list, int toRemove)
        {
            list.Remove(toRemove);
        }
        
        /// <summary>
        /// Tries to parse an Enum, if not successful, it returns null instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T? ParseNullable<T>(string input) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(input) && Enum.GetNames(typeof(T)).Any((string e) => e.Trim().ToUpperInvariant() == input.Trim().ToUpperInvariant()))
            {
                return (T)Enum.Parse(typeof(T), input, ignoreCase: true);
            }
            return null;
        }

        /// <summary>
        /// Equivalent to <see cref="Enum.Parse(Type, string, bool)"/>, with the painful parts taken care of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Parse<T>(string input) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), input, ignoreCase: true);
        }

        /// <summary>
        /// Returns the first keycode pressed in the same frame as <see cref="GetKeyStroke(int, Keys)"/> was called in.
        /// </summary>
        /// <param name="type">
        /// <list type="bullet">
        /// <item>0 = <see cref="InputManager.IsKeyDown(Keys)"/></item>
        /// <item>1 = <see cref="InputManager.IsKeyPressed(Keys)"/></item>
        /// <item>2 = <see cref="InputManager.IsKeyReleased(Keys)"/></item>
        /// </list>
        /// </param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Keys GetKeyStroke(int type, InputManager input)
        {
            foreach (Keys value in Enum.GetValues(typeof(Keys)))
            {
                
                if ((type == 0 && input.IsKeyDown(value)) || (type == 1 && input.IsKeyPressed(value)) || (type == 2 && input.IsKeyReleased(value)))
                {
                    return value;
                }
            }
            return Keys.None;
        }

        /// <summary>
        /// Checks if the current thread is the main thread.
        /// </summary>
        public static void CheckForMainThread()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                !Thread.CurrentThread.IsBackground && !Thread.CurrentThread.IsThreadPoolThread && Thread.CurrentThread.IsAlive)
            {
                MethodInfo correctEntryMethod = Assembly.GetEntryAssembly().EntryPoint;
                StackTrace trace = new StackTrace();
                StackFrame[] frames = trace.GetFrames();
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    MethodBase method = frames[i].GetMethod();
                    if (correctEntryMethod == method)
                    {
                        return;
                    }
                }
            }

            throw new Exception("Not on main thread!");
        }

        /// <summary>
        /// Returns the <see cref="RegistryKey"/> based on a key path and registy hive.
        /// </summary>
        /// <param name="RegKeyPath"></param>
        /// <param name="CategorySearch"></param>
        /// <returns></returns>
        public static RegistryKey RegistryFromPath(string RegKeyPath, RegistryHive CategorySearch)
        {
            switch (CategorySearch)
            {
                default:
                    return Registry.CurrentUser.OpenSubKey(RegKeyPath);
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine.OpenSubKey(RegKeyPath);
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot.OpenSubKey(RegKeyPath);
                case RegistryHive.CurrentConfig:
                    return Registry.LocalMachine.OpenSubKey(RegKeyPath);
                case RegistryHive.PerformanceData:
                    return Registry.PerformanceData.OpenSubKey(RegKeyPath);
                case RegistryHive.Users:
                    return Registry.Users.OpenSubKey(RegKeyPath);
            }
        }

        /// <summary>
        /// Attempts to return all value names from a registry path and registry hive.
        /// </summary>
        /// <param name="RegKeyPath"></param>
        /// <param name="CategorySearch"></param>
        /// <returns></returns>
        public static string[] GetSubkeyNamesFromKey(string RegKeyPath, RegistryHive CategorySearch)
        {
            return RegistryFromPath(RegKeyPath, CategorySearch)?.GetValueNames() ?? null;
        }

        /// <summary>
        /// Equivalent to <see cref="float.Parse(string, IFormatProvider)"/> where <see cref="IFormatProvider"/>
        /// is <see cref="CultureInfo"/>.InvariantCulture.NumberFormat.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <returns></returns>
        public static float ToSingle(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Returns a list of rotations based around the amount given; amount 2 would return 0 and 180, 3 would return 0, 120 and 240, 4 would return 0, 80, 160 and 240, etc...
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="amount"></param>
        /// <param name="maxRotation"></param>
        /// <returns></returns>
        public static float[] Get2DRotationsAroundPoint(float offset, int amount, float maxRotation = 360)
        {
            float[] toReturn = new float[amount];
            for(int i = 0; i < amount; i++)
            {
                toReturn[i] = ((maxRotation / amount) * i) + offset;
            }

            return toReturn;
        }

        /// <summary>
        /// Returns the parent of the collider. If collider is already the parent, it returns the collider's GameObject instead.
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static Entity GetParent(PhysicsComponent collider)
        {
            try
            {
                Entity result = collider.Entity;
                GetOldestParent(result.Transform);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Populates a list with all children of a parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="list"></param>
        public static void GetAllChildren(TransformComponent parent, ref List<TransformComponent> list)
        {
            foreach (TransformComponent child in parent.Children)
            {
                list.Add(child);
                GetAllChildren(child, ref list);
            }
        }

        /// <summary>
        /// Populates a list with all children of a parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="list"></param>
        public static void GetAllChildren(TransformComponent parent, ref List<Entity> list)
        {
            foreach (TransformComponent child in parent.Children)
            {
                list.Add(child.Entity);
                GetAllChildren(child, ref list);
            }
        }

        /// <summary>
        /// Gets all children of a parent who match a specified condition.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<TransformComponent> GetChildren(TransformComponent parent, Predicate<TransformComponent> match)
        {
            List<TransformComponent> toReturn = new();
            List<TransformComponent> used = new (parent.Children.Count);

            GetAllChildren(parent, ref used);

            foreach (TransformComponent transform in used)
            {
                if (match(transform))
                {
                    toReturn.Add(transform);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all children of a parent who match a specified condition.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<Entity> GetChildren(TransformComponent parent, Predicate<Entity> match)
        {
            List<Entity> toReturn = new();
            List<TransformComponent> used = new (parent.Children.Count);

            GetAllChildren(parent, ref used);

            foreach (TransformComponent transform in used)
            {
                if (match(transform.Entity))
                {
                    toReturn.Add(transform.Entity);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns the parent gameobject of the given <see cref="Entity"/>. If the parent is null, it will return the entity itself.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Entity EntityParent(Entity entity)
        {
            Entity result = entity;
            if (entity.Transform.Parent != null)
            {
                result = entity.Transform.Parent.Entity;
            }
            return result;
        }

        /// <summary>
        /// Gets the oldest parent inside the hierarchy.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static TransformComponent GetOldestParent(TransformComponent child)
        {
            try
            {
                TransformComponent toReturn = child;
                while (toReturn.Parent != null)
                {
                    toReturn = toReturn.Parent;
                }

                return toReturn;
            }
            catch
            {
                return child;
            }
        }

        /// <summary>
        /// Returns the oldest parent in the hierarchy of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Entity GetOldestParent(Entity entity)
        {
            TransformComponent oldest = GetOldestParent(entity.Transform);

            return oldest.Entity;
        }

        /// <summary>
        /// Calls a method by name inside a static class.
        /// </summary>
        /// <param name="classToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <param name="specificBind"></param>
        public static void CallStaticMethod(Type classToCheck, string methodName, object[] args, BindingFlags specificBind)
        {
            try
            {
                MethodInfo method = classToCheck.GetMethod(methodName, specificBind);
                method.Invoke(null, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

#pragma warning disable CS1591
        /*
         * Copyright (c) 2018 Simon Cropp
         *
         * Permission is hereby granted, free of charge, to any person obtaining a copy
         * of this software and associated documentation files (the "Software"), to deal
         * in the Software without restriction, including without limitation the rights
         * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
         * copies of the Software, and to permit persons to whom the Software is
         * furnished to do so, subject to the following conditions:
         *
         * The above copyright notice and this permission notice shall be included in all
         * copies or substantial portions of the Software.
         * 
         * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
         * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
         * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
         * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
         * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
         * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
         * SOFTWARE.
        */
        /// <summary>
        /// A class used for clipboard functionality; All contents are copied from the <see href="https://github.com/CopyText/TextCopy"> TextCopy github</see>
        /// </summary>
        public static class Clipboard
        {
            /// <summary>
            /// The windows clipboard.
            /// </summary>
            public static class Windows
            {
                public static async Task SetTextAsync(string text, CancellationToken cancellation)
                {
                    await TryOpenClipboardAsync(cancellation);

                    InnerSet(text);
                }

                public static void SetText(string text)
                {
                    TryOpenClipboard();

                    InnerSet(text);
                }

                static void InnerSet(string text)
                {
                    EmptyClipboard();
                    IntPtr hGlobal = default;
                    try
                    {
                        var bytes = (text.Length + 1) * 2;
                        hGlobal = Marshal.AllocHGlobal(bytes);

                        if (hGlobal == default)
                        {
                            ThrowWin32();
                        }

                        var target = GlobalLock(hGlobal);

                        if (target == default)
                        {
                            ThrowWin32();
                        }

                        try
                        {
                            Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                        }
                        finally
                        {
                            GlobalUnlock(target);
                        }

                        if (SetClipboardData(cfUnicodeText, hGlobal) == default)
                        {
                            ThrowWin32();
                        }

                        hGlobal = default;
                    }
                    finally
                    {
                        if (hGlobal != default)
                        {
                            Marshal.FreeHGlobal(hGlobal);
                        }

                        CloseClipboard();
                    }
                }

                static async Task TryOpenClipboardAsync(CancellationToken cancellation)
                {
                    var num = 10;
                    while (true)
                    {
                        if (OpenClipboard(default))
                        {
                            break;
                        }

                        if (--num == 0)
                        {
                            ThrowWin32();
                        }

                        await Task.Delay(100, cancellation);
                    }
                }

                static void TryOpenClipboard()
                {
                    var num = 10;
                    while (true)
                    {
                        if (OpenClipboard(default))
                        {
                            break;
                        }

                        if (--num == 0)
                        {
                            ThrowWin32();
                        }

                        Thread.Sleep(100);
                    }
                }

                public static async Task<string> GetTextAsync(CancellationToken cancellation)
                {
                    if (!IsClipboardFormatAvailable(cfUnicodeText))
                    {
                        return null;
                    }
                    await TryOpenClipboardAsync(cancellation);

                    return InnerGet();
                }

                public static string GetText()
                {
                    if (!IsClipboardFormatAvailable(cfUnicodeText))
                    {
                        return null;
                    }
                    TryOpenClipboard();

                    return InnerGet();
                }

                static string InnerGet()
                {
                    IntPtr handle = default;

                    IntPtr pointer = default;
                    try
                    {
                        handle = GetClipboardData(cfUnicodeText);
                        if (handle == default)
                        {
                            return null;
                        }

                        pointer = GlobalLock(handle);
                        if (pointer == default)
                        {
                            return null;
                        }

                        var size = GlobalSize(handle);
                        var buff = new byte[size];

                        Marshal.Copy(pointer, buff, 0, size);

                        return Encoding.Unicode.GetString(buff).TrimEnd('\0');
                    }
                    finally
                    {
                        if (pointer != default)
                        {
                            GlobalUnlock(handle);
                        }

                        CloseClipboard();
                    }
                }

                const uint cfUnicodeText = 13;

                static void ThrowWin32()
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                [DllImport("User32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static extern bool IsClipboardFormatAvailable(uint format);

                [DllImport("User32.dll", SetLastError = true)]
                static extern IntPtr GetClipboardData(uint uFormat);

                [DllImport("kernel32.dll", SetLastError = true)]
                static extern IntPtr GlobalLock(IntPtr hMem);

                [DllImport("kernel32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static extern bool GlobalUnlock(IntPtr hMem);

                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static extern bool OpenClipboard(IntPtr hWndNewOwner);

                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static extern bool CloseClipboard();

                [DllImport("user32.dll", SetLastError = true)]
                static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

                [DllImport("user32.dll")]
                static extern bool EmptyClipboard();

                [DllImport("Kernel32.dll", SetLastError = true)]
                static extern int GlobalSize(IntPtr hMem);
            }

            /// <summary>
            /// The Mac OS clipboard.
            /// </summary>
            public static class OsX
            {
                static IntPtr nsString = objc_getClass("NSString");
                static IntPtr nsPasteboard = objc_getClass("NSPasteboard");
                static IntPtr nsStringPboardType;
                static IntPtr utfTextType;
                static IntPtr generalPasteboard;
                static IntPtr initWithUtf8Register = sel_registerName("initWithUTF8String:");
                static IntPtr allocRegister = sel_registerName("alloc");
                static IntPtr setStringRegister = sel_registerName("setString:forType:");
                static IntPtr stringForTypeRegister = sel_registerName("stringForType:");
                static IntPtr utf8Register = sel_registerName("UTF8String");
                static IntPtr generalPasteboardRegister = sel_registerName("generalPasteboard");
                static IntPtr clearContentsRegister = sel_registerName("clearContents");

                static OsX()
                {
                    utfTextType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "public.utf8-plain-text");
                    nsStringPboardType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "NSStringPboardType");

                    generalPasteboard = objc_msgSend(nsPasteboard, generalPasteboardRegister);
                }

                public static string GetText()
                {
                    var ptr = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
                    var charArray = objc_msgSend(ptr, utf8Register);
                    return Marshal.PtrToStringAnsi(charArray);
                }

                public static Task<string> GetTextAsync(CancellationToken cancellation)
                {
                    return Task.FromResult(GetText());
                }

                public static void SetText(string text)
                {
                    IntPtr str = default;
                    try
                    {
                        str = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, text);
                        objc_msgSend(generalPasteboard, clearContentsRegister);
                        objc_msgSend(generalPasteboard, setStringRegister, str, utfTextType);
                    }
                    finally
                    {
                        if (str != default)
                        {
                            objc_msgSend(str, sel_registerName("release"));
                        }
                    }
                }

                public static Task SetTextAsync(string text, CancellationToken cancellation)
                {
                    SetText(text);
                    return Task.CompletedTask;
                }

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr objc_getClass(string className);

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

                [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
                static extern IntPtr sel_registerName(string selectorName);
            }

            /// <summary>
            /// The Linux clipboard.
            /// </summary>
            public static class Linux
            {
                static bool isWsl;

                static Linux()
                {
                    isWsl = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null;
                }

                public static async Task SetTextAsync(string text, CancellationToken cancellation)
                {
                    var tempFileName = Path.GetTempFileName();
                    await File.WriteAllTextAsync(tempFileName, text, cancellation);

                    if (cancellation.IsCancellationRequested)
                    {
                        return;
                    }

                    InnerSetText(tempFileName);
                }

                public static void SetText(string text)
                {
                    var tempFileName = Path.GetTempFileName();
                    File.WriteAllText(tempFileName, text);
                    InnerSetText(tempFileName);
                }

                static void InnerSetText(string tempFileName)
                {
                    try
                    {
                        if (isWsl)
                        {
                            BashRunner.Run($"cat {tempFileName} | clip.exe ");
                        }
                        else
                        {
                            BashRunner.Run($"cat {tempFileName} | xsel -i --clipboard ");
                        }
                    }
                    finally
                    {
                        File.Delete(tempFileName);
                    }
                }

                public static string GetText()
                {
                    var tempFileName = Path.GetTempFileName();
                    try
                    {
                        InnerGetText(tempFileName);
                        return File.ReadAllText(tempFileName);
                    }
                    finally
                    {
                        File.Delete(tempFileName);
                    }
                }

                public static async Task<string> GetTextAsync(CancellationToken cancellation)
                {
                    var tempFileName = Path.GetTempFileName();
                    try
                    {
                        InnerGetText(tempFileName);
                        return await File.ReadAllTextAsync(tempFileName, cancellation);
                    }
                    finally
                    {
                        File.Delete(tempFileName);
                    }
                }

                static void InnerGetText(string tempFileName)
                {
                    if (isWsl)
                    {
                        BashRunner.Run($"powershell.exe Get-Clipboard  > {tempFileName}");
                    }
                    else
                    {
                        BashRunner.Run($"xsel -o --clipboard  > {tempFileName}");
                    }
                }
            }

            private static class BashRunner
            {
                public static string Run(string commandLine)
                {
                    StringBuilder errorBuilder = new();
                    StringBuilder outputBuilder = new();
                    var arguments = $"-c \"{commandLine}\"";
                    using Process process = new()
                    {
                        StartInfo = new()
                        {
                            FileName = "bash",
                            Arguments = arguments,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = false,
                        }
                    };
                    process.Start();
                    process.OutputDataReceived += (_, args) => { outputBuilder.AppendLine(args.Data); };
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += (_, args) => { errorBuilder.AppendLine(args.Data); };
                    process.BeginErrorReadLine();
                    if (!DoubleWaitForExit(process))
                    {
                        var timeoutError = $@"Process timed out. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
                        throw new(timeoutError);
                    }
                    if (process.ExitCode == 0)
                    {
                        return outputBuilder.ToString();
                    }

                    var error = $@"Could not execute process. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
                    throw new(error);
                }

                //To work around https://github.com/dotnet/runtime/issues/27128
                static bool DoubleWaitForExit(Process process)
                {
                    var result = process.WaitForExit(500);
                    if (result)
                    {
                        process.WaitForExit();
                    }
                    return result;
                }
            }
        }
#pragma warning restore CS1591
        /// <summary>
        /// Sets the text/title of an application widow. Argument hwnd is a window, 
        /// which can be found using <see cref="FindWindow(string, string)"/> or
        /// <see cref="GetActiveWindow"/>.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lp"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, string lp);
        /// <summary>
        /// Returns all info on a window using <see cref="IntPtr"/>; See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowa
        /// for more info.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);

        /// <summary>
        /// Returns the currently active window.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Sets the application's title to a given string.
        /// </summary>
        /// <param name="to"></param>
        public static void SetWindowText(string to)
        {
            SetWindowText(GetActiveWindow(), to);
        }

        /// <summary>
        /// Returns a sprite from a multisprite spritesheet; 
        /// SpriteSheetName is the name of the file, spriteName is the name of the sprite itself.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="SpriteSheetName"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite LoadSpriteFromSpriteSheet(ContentManager content, string SpriteSheetName, string spriteName)
        {
            SpriteSheet source = content.Load<SpriteSheet>(SpriteSheetName);
            
            return source.Sprites.Find((Sprite s) => s.Name == spriteName);
        }

        /// <summary>
        /// Returns true if type is subclass of baseType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
            {
                return false;
            }
            if (!baseType.IsGenericType)
            {
                if (!type.IsGenericType)
                {
                    return type.IsSubclassOf(baseType);
                }
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }
            type = type.BaseType;
            Type typeFromHandle = typeof(object);
            while (type != typeFromHandle && type != null)
            {
                Type left = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (left == baseType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is string.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static string EnumerableConcat(string origin, IEnumerable<string> adders)
        {
            if (adders == null)
            {
                return origin;
            }
            foreach (string adder in adders)
            {
                origin += adder;
            }
            return origin;
        }
        /// <summary>
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is string.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static string EnumerableConcat(IEnumerable<string> adders)
            => EnumerableConcat(string.Empty, adders);

        /// <summary>
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is float.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static float EnumerableConcat(float origin, IEnumerable<float> adders)
        {
            float toReturn = origin;
            foreach (float f in adders)
                toReturn += f;

            return toReturn;
        }
        /// <summary>
        /// Returns all values merged into one from an <see cref="IEnumerable{T}"/> where T is float.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static float EnumerableConcat(IEnumerable<float> adders)
            => EnumerableConcat(0, adders);

        /// <summary>
        /// Merges all int values of a list into one value.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static int EnumerableConcat(int origin, IEnumerable<int> adders)
        {
            if (adders == null)
            {
                return origin;
            }
            foreach (int adder in adders)
            {
                origin += adder;
            }
            return origin;
        }
        /// <summary>
        /// Merges all int values of a list into one value.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static int EnumerableConcat(IEnumerable<int> adders)
           => EnumerableConcat(0, adders);

        /// <summary>
        /// Converts a given int value into an Enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T ConvertEnum<T>(int i) where T : struct, IConvertible
        {
            return (T)(object)i;
        }
    }
}