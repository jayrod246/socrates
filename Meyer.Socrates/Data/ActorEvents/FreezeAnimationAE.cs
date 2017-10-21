using Meyer.Socrates.IO;
using Meyer.Socrates.Data;
using System;

namespace Meyer.Socrates.Data.ActorEvents
{
    /// <summary>
    /// Actor Event that freezes or unfreezes the current Action animation.
    /// </summary>
    [ActorEventType(ActorEventType.FreezeAnimation)]
    public class FreezeAnimationAE: VirtualActorEvent
    {
        /// <summary>
        /// When set to true, the current action will stop animating.
        /// </summary>
        public bool IsFrozen { get => GetValue<bool>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            // Length: 24
            IsFrozen = c.Read<Int32>() != 0;
        }
        protected override void Write(IDataWriteContext c)
        {
            c.Write(IsFrozen ? 1 : 0);
        }
    }
}
