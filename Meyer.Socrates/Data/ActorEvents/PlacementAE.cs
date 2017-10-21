using Meyer.Socrates.IO;
using Meyer.Socrates.Data;

namespace Meyer.Socrates.Data.ActorEvents
{
    [ActorEventType(ActorEventType.Placement)]
    public class PlacementAE: VirtualActorEvent
    {
        // TODO: Confirm that AEPlacement.Position is actually relative to ACTR.Position.
        /// <summary>
        /// The position at which the object will be placed at, relative to <seealso cref="ACTR.Position"/>.
        /// </summary>
        public BrVector3 Position { get=> GetValue<BrVector3>(); set=> SetValue(value); }

        /// <summary>
        /// The absolute rotation the object will have after being placed.
        /// </summary>
        public BrEuler Rotation { get => GetValue<BrEuler>(); set => SetValue(value); }

        protected override void Read(IDataReadContext c)
        {
            Position = c.Read<BrVector3>();
            Rotation = c.Read<BrEuler>();
        }

        protected override void Write(IDataWriteContext c)
        {
            c.Write(Position);
            c.Write(Rotation);
        }
    }
}
