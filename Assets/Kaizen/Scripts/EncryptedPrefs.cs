using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kaizen
{
    public sealed class EncryptedPrefs
    {
        private static AesManaged aesManaged;

        private static Dictionary<string, string> dict;

        public static void Initialize(string encryptionKey)
        {
            dict = new Dictionary<string, string>();

            aesManaged = new AesManaged()
            {
                Key = Encoding.UTF8.GetBytes(encryptionKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }

        private static string Encrypt(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            ICryptoTransform transform = aesManaged.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Convert.ToBase64String(result, 0, result.Length);
        }

        private static string Decrypt(string data)
        {
            byte[] dataBytes = Convert.FromBase64String(data);
            ICryptoTransform transform = aesManaged.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Encoding.UTF8.GetString(result, 0, result.Length);
        }

        /// <summary>
        /// Set a string data value. Automatically register the entry if doesn't exist.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetString(string key, string value)
        {
            dict[Encrypt(key)] = Encrypt(value);
        }

        /// <summary>
        /// Get registered string data.
        /// </summary>
        /// <param name="key">The identifier registered for a specific value.</param>
        public static string GetString(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return Decrypt(value);
            }
            return string.Empty;
        }

        /// <summary>
        /// Set an integer value.  Automatically register a new integer entry if doesn't exist.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetInt(string key, int value)
        {
            dict[Encrypt(key)] = Encrypt(value.ToString());
        }

        /// <summary>
        /// Add value to integer entry. Automatically register the entry if doesn't exist.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be added.</param>
        /// <returns>New value.</returns>
        public static int AddInt(string key, int value)
        {
            var newValue = GetInt(key) + value;
            SetInt(key, newValue);
            return newValue;
        }

        /// <summary>
        /// Gets integer data. Return 0 if entry doesn't exist.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static int GetInt(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return int.Parse(Decrypt(value));
            }
            return 0;
        }

        /// <summary>
        /// Sets float data. Automatically register the entry if doesn't exist.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetFloat(string key, float value)
        {
            dict[Encrypt(key)] = Encrypt(value.ToString());
        }

        /// <summary>
        /// Adds float data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be added.</param>
        /// <returns>New value.</returns>
        public static float AddFloat(string key, float value)
        {
            var newValue = GetFloat(key) + value;
            SetFloat(key, newValue);
            return newValue;
        }

        /// <summary>
        /// Gets float data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static float GetFloat(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return float.Parse(Decrypt(value));
            }
            return 0.0f;
        }

        /// <summary>
        /// Sets boolean data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetBool(string key, bool value)
        {
            dict[Encrypt(key)] = Encrypt(value.ToString());
        }

        /// <summary>
        /// Gets boolean data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static bool GetBool(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return bool.Parse(Decrypt(value));
            }
            return false;
        }

        /// <summary>
        /// Sets long data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetLong(string key, long value)
        {
            dict[Encrypt(key)] = Encrypt(value.ToString());
        }

        /// <summary>
        /// Adds long data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be added.</param>
        public static long AddLong(string key, long value)
        {
            var newValue = GetLong(key) + value;
            SetLong(key, newValue);
            return newValue;
        }

        /// <summary>
        /// Gets long data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static long GetLong(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return long.Parse(Decrypt(value));
            }
            return 0;
        }

        /// <summary>
        /// Sets JSON data.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        /// <param name="value">The value that will be stored.</param>
        public static void SetJson(string key, object value)
        {
            dict[Encrypt(key)] = Encrypt(JsonUtility.ToJson(value));
        }

        /// <summary>
        /// Gets JSON data as dictionary.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static object GetJson<T>(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return JsonUtility.FromJson<T>(Decrypt(value));
            }
            return null;
        }

        public static void SetList<T>(string key, List<T> value)
        {
            dict[Encrypt(key)] = Encrypt(JsonUtility.ToJson(value));
        }

        /// <summary>
        /// Gets JSON data as dictionary.
        /// </summary>
        /// <param name="key">The identifier for addressing the specific value.</param>
        public static List<T> GetList<T>(string key)
        {
            if (dict.TryGetValue(Encrypt(key), out string value))
            {
                return JsonUtility.FromJson<List<T>>(Decrypt(value));
            }
            return null;
        }

        /// <summary>
        /// Checks whether the registered key exists or not.
        /// </summary>
        /// <param name="key">The identifier to be checked.</param>
        public static bool HasKey(string key)
        {
            return dict.ContainsKey(Encrypt(key));
        }

        /// <summary>
        /// Deletes a registered value by a registered key.
        /// </summary>
        /// <param name="key">The identifier for the targeted value.</param>
        /// <returns></returns>
        public static bool DeleteKey(string key)
        {
            return dict.Remove(Encrypt(key));
        }

        /// <summary>
        /// Clears all encrypted data.
        /// </summary>
        public static void Clear()
        {
            dict.Clear();
        }

        public static byte[] Serialize()
        {
            if (dict == null)
                return null;

            var sb = new StringBuilder();
            foreach (var kvp in dict)
            {
                sb.AppendLine(kvp.Key);
                sb.AppendLine(kvp.Value);
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return dataBytes;
        }

        public static void Deserialize(byte[] data)
        {
            dict.Clear();

            var result = Encoding.UTF8.GetString(data, 0, data.Length);
            var sr = new StringReader(result);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var key = line;
                var value = sr.ReadLine();
                dict.Add(key, value);
            }
        }

        public static void SaveProgress()
        {
            var bytes = Serialize();
            if (bytes != null)
            {
                try
                {
                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "save.dat"), bytes);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while saving data: " + e.Message);
                }
            }
        }

        public static void LoadProgress()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, "save.dat");
            if (File.Exists(dataPath))
            {
                try
                {
                    Deserialize(File.ReadAllBytes(dataPath));
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing save file: " + e.Message);
                }
            }
        }
    }
}