namespace Meyer.Socrates.Services
{
    using Meyer.Socrates.Data.Sections;
    using System;
    using System.Collections.Generic;

    public static partial class VirtualSectionRegistry
    {
        private static readonly Dictionary<string, Type> keyToType = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, string> typeToKey = new Dictionary<Type, string>();

        public static void Register<T>(string key) where T : VirtualSection
        {
            keyToType[key] = typeof(T);
            typeToKey[typeof(T)] = key;
        }

        public static Type ResolveTypeFromKey(string key)
        {
            return keyToType[key];
        }

        public static string ResolveKeyFromType<T>() where T : VirtualSection => typeToKey[typeof(T)];

        public static string ResolveKeyFromType(Type type) => typeToKey[type];

        public static bool TryResolveTypeFromKey(string key, out Type type)
        {
            return keyToType.TryGetValue(key, out type);
        }

        public static bool TryResolveTypeFromKey(Type type, out string key)
        {
            return typeToKey.TryGetValue(type, out key);
        }

        public static bool TryResolveTypeFromKey<T>(out string key) where T : VirtualSection
        {
            return typeToKey.TryGetValue(typeof(T), out key);
        }
    }
}
