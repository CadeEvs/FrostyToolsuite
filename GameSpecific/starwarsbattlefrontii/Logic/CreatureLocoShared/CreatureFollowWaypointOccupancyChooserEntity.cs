using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointOccupancyChooserEntityData))]
	public class CreatureFollowWaypointOccupancyChooserEntity : CreatureFollowBase, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointOccupancyChooserEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointOccupancyChooserEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointOccupancyChooserEntityData;
		public override string DisplayName => "CreatureFollowWaypointOccupancyChooser";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureFollowWaypointOccupancyChooserEntity(FrostySdk.Ebx.CreatureFollowWaypointOccupancyChooserEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

