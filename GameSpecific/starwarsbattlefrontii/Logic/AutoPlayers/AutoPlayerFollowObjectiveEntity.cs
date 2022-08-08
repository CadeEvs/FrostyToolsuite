using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerFollowObjectiveEntityData))]
	public class AutoPlayerFollowObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerFollowObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerFollowObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerFollowObjectiveEntityData;
		public override string DisplayName => "AutoPlayerFollowObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerFollowObjectiveEntity(FrostySdk.Ebx.AutoPlayerFollowObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

