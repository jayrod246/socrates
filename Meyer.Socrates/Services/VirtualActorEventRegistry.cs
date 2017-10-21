namespace Meyer.Socrates.Services
{
    using Meyer.Socrates.Data.ActorEvents;
    using Meyer.Socrates.Data;
    using System;
    using System.Collections.Generic;

    public static partial class VirtualActorEventRegistry
    {
        private static readonly Dictionary<ActorEventType, Type> keyToType = new Dictionary<ActorEventType, Type>();
        private static readonly Dictionary<Type, ActorEventType> typeToKey = new Dictionary<Type, ActorEventType>();

        public static void Register<T>(ActorEventType key) where T : VirtualActorEvent
        {
            keyToType[key] = typeof(T);
            typeToKey[typeof(T)] = key;
        }

        public static Type ResolveTypeFromKey(ActorEventType key)
        {
            return keyToType[key];
        }

        public static ActorEventType ResolveKeyFromType<T>() where T : VirtualActorEvent => typeToKey[typeof(T)];

        public static bool TryResolveTypeFromKey(ActorEventType key, out Type type)
        {
            return keyToType.TryGetValue(key, out type);
        }

        public static bool TryResolveTypeFromKey(Type type, out ActorEventType key)
        {
            return typeToKey.TryGetValue(type, out key);
        }

        public static bool TryResolveTypeFromKey<T>(out ActorEventType key) where T : VirtualActorEvent
        {
            return typeToKey.TryGetValue(typeof(T), out key);
        }
    }
}
