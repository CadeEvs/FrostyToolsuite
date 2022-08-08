using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerDefendObjectiveEntityData))]
	public class AutoPlayerDefendObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerDefendObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerDefendObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerDefendObjectiveEntityData;
		public override string DisplayName => "AutoPlayerDefendObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerDefendObjectiveEntity(FrostySdk.Ebx.AutoPlayerDefendObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

