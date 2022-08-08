using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FriendsStatsEntityData))]
	public class FriendsStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FriendsStatsEntityData>
	{
		public new FrostySdk.Ebx.FriendsStatsEntityData Data => data as FrostySdk.Ebx.FriendsStatsEntityData;
		public override string DisplayName => "FriendsStats";

		public FriendsStatsEntity(FrostySdk.Ebx.FriendsStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

