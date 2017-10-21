using Meyer.Socrates.IO;
using Meyer.Socrates.Data;
using System;

namespace Meyer.Socrates.Data.ActorEvents
{
    /// <summary>
    /// Actor Event that changes the action.
    /// </summary>
    [ActorEventType(ActorEventType.SetAnimation)]
    public class SetAnimationAE: VirtualActorEvent
    {
        /// <summary>
        /// The reference ID to the action changing to.
        /// </summary>
        public UInt32 Action { get => GetValue<UInt32>(); set => SetValue(value); }

        /// <summary>
        /// The frame within the action to begin at.
        /// </summary>
        public Int32 Frame { get => GetValue<Int32>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            Action = c.Read<UInt32>();
            Frame = c.Read<Int32>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(Action);
            c.Write(Frame);
        }
    }
}
