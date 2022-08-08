using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointProviderEntityData))]
	public class CreatureFollowWaypointProviderEntity : CreatureBaseWaypointProviderEntity, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointProviderEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointProviderEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointProviderEntityData;
		public override string DisplayName => "CreatureFollowWaypointProvider";

		public CreatureFollowWaypointProviderEntity(FrostySdk.Ebx.CreatureFollowWaypointProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

