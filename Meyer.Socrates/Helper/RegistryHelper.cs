namespace Meyer.Socrates.Helper
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;

    internal static class RegistryHelper
    {
        internal static readonly RegistryView RegistryView = RegistryView.Registry32; // IntPtr.Size == 4 ? RegistryView.Registry64 : RegistryView.Registry32;
        internal static readonly IRegistryHive LocalMachine = new Hive(() => RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView));
        internal static readonly IRegistryHive ClassesRoot = new Hive(() => RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView));

        internal static T GetValue<T>(string keyName, string valueName, T defaultValue)
        {
            int i = keyName.IndexOf('\\');
            if (TryParseHive(keyName.Substring(0, i), out var hive))
            {
                using (var key = hive.OpenSubKey(keyName.Substring(i + 1)))
                    return (T)key.GetValue(valueName, defaultValue);
            }
            return defaultValue;
        }

        internal static object GetValue(string keyName, string valueName, object defaultValue)
        {
            int i = keyName.IndexOf('\\');
            if (TryParseHive(keyName.Substring(0, i), out var hive))
            {
                using (var key = hive.OpenSubKey(keyName.Substring(i + 1)))
                    return key.GetValue(valueName, defaultValue);
            }
            return defaultValue;
        }

        internal static string[] GetValueNames(string keyName)
        {
            int i = keyName.IndexOf('\\');
            if (TryParseHive(keyName.Substring(0, i), out var hive))
            {
                using (var key = hive.OpenSubKey(keyName.Substring(i + 1)))
                    return key.GetValueNames();
            }
            return Array.Empty<string>();
        }

        internal static void SetValue(string keyName, string valueName, object value)
        {
            int i = keyName.IndexOf('\\');
            if (TryParseHive(keyName.Substring(0, i), out var hive))
            {
                using (var key = hive.OpenSubKey(keyName.Substring(i + 1)))
                    key.SetValue(valueName, value);
            }
        }

        internal static void SetValue<T>(string keyName, string valueName, T value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    SetValue(keyName, valueName, value, RegistryValueKind.String);
                    break;
                default:
                    SetValue(keyName, valueName, (object)value);
                    break;
            }
        }

        internal static void SetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
        {
            int i = keyName.IndexOf('\\');
            if (TryParseHive(keyName.Substring(0, i), out var hive))
            {
                using (var key = hive.OpenSubKey(keyName.Substring(i + 1)))
                    key.SetValue(valueName, value, valueKind);
            }
        }

        private class Hive: IRegistryHive
        {
            private Func<RegistryKey> factory;

            public Hive(Func<RegistryKey> factory)
            {
                this.factory = factory;
            }

            public RegistryKey OpenSubKey(string name)
            {
                using (var baseKey = factory.Invoke())
                {
                    return baseKey.OpenSubKey(name) ?? throw new InvalidOperationException("Registry key does not exist, or it could not be opened.") { Data = { { "RegistryKeyName", $"{baseKey.Name}\\{name}" } } };
                }
            }
        }

        internal static bool TryParseHive(string s, out IRegistryHive result)
        {
            result = null;
            s = s.ToUpperInvariant();
            switch (s)
            {
                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    result = LocalMachine;
                    break;
            }

            return result != null;
        }
    }

    internal interface IRegistryHive
    {
        /// <summary>
        /// Retrieves a subkey.
        /// </summary>
        /// <param name="name">The name or path of the subkey to open.</param>
        /// <exception cref="InvalidOperationException">name does not exist in registry, or could not be opened.</exception>
        /// <returns>The subkey requested.</returns>
        RegistryKey OpenSubKey(string name);
    }
}
