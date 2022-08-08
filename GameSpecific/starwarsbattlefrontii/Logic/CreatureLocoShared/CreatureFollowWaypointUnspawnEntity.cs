using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointUnspawnEntityData))]
	public class CreatureFollowWaypointUnspawnEntity : CreatureFollowBase, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointUnspawnEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointUnspawnEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointUnspawnEntityData;
		public override string DisplayName => "CreatureFollowWaypointUnspawn";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureFollowWaypointUnspawnEntity(FrostySdk.Ebx.CreatureFollowWaypointUnspawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

