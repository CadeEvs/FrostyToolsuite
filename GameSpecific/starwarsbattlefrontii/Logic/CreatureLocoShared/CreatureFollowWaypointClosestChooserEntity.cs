using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointClosestChooserEntityData))]
	public class CreatureFollowWaypointClosestChooserEntity : CreatureFollowBase, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointClosestChooserEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointClosestChooserEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointClosestChooserEntityData;
		public override string DisplayName => "CreatureFollowWaypointClosestChooser";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureFollowWaypointClosestChooserEntity(FrostySdk.Ebx.CreatureFollowWaypointClosestChooserEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

