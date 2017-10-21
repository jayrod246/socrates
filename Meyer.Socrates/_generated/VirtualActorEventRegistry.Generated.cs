/*
	This code was generated 10/07/2017 02:31:35
*/

using Meyer.Socrates.Data.ActorEvents;
using Meyer.Socrates.Data;

namespace Meyer.Socrates.Services
{
	public static partial class VirtualActorEventRegistry
	{
		static VirtualActorEventRegistry()
        {
			Register<FreezeAnimationAE>(ActorEventType.FreezeAnimation);
			Register<PlacementAE>(ActorEventType.Placement);
			Register<SetAnimationAE>(ActorEventType.SetAnimation);
			Register<SetCostumeAE>(ActorEventType.SetCostume);
        }
	}
}
