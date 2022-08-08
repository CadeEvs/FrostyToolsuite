using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MoverFollowLeaderEntityData))]
	public class MoverFollowLeaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MoverFollowLeaderEntityData>
	{
		public new FrostySdk.Ebx.MoverFollowLeaderEntityData Data => data as FrostySdk.Ebx.MoverFollowLeaderEntityData;
		public override string DisplayName => "MoverFollowLeader";

		public MoverFollowLeaderEntity(FrostySdk.Ebx.MoverFollowLeaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

