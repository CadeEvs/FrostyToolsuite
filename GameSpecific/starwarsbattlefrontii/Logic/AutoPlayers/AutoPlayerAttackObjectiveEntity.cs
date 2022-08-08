using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerAttackObjectiveEntityData))]
	public class AutoPlayerAttackObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerAttackObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerAttackObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerAttackObjectiveEntityData;
		public override string DisplayName => "AutoPlayerAttackObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerAttackObjectiveEntity(FrostySdk.Ebx.AutoPlayerAttackObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

