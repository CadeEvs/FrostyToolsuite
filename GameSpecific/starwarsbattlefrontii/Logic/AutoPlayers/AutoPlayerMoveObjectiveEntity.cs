using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerMoveObjectiveEntityData))]
	public class AutoPlayerMoveObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerMoveObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerMoveObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerMoveObjectiveEntityData;
		public override string DisplayName => "AutoPlayerMoveObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerMoveObjectiveEntity(FrostySdk.Ebx.AutoPlayerMoveObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

