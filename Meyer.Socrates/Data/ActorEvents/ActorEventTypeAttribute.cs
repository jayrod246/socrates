namespace Meyer.Socrates.Data.ActorEvents
{
    using Meyer.Socrates.Data;
    using System;

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ActorEventTypeAttribute: Attribute
    {
        readonly ActorEventType type;

        public ActorEventTypeAttribute(ActorEventType type)
        {
            this.type = type;
        }

        public ActorEventType Type
        {
            get { return type; }
        }
    }
}
