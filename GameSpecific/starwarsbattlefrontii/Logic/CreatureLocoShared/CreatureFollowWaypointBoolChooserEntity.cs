using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointBoolChooserEntityData))]
	public class CreatureFollowWaypointBoolChooserEntity : CreatureFollowBase, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointBoolChooserEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointBoolChooserEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointBoolChooserEntityData;
		public override string DisplayName => "CreatureFollowWaypointBoolChooser";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureFollowWaypointBoolChooserEntity(FrostySdk.Ebx.CreatureFollowWaypointBoolChooserEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

