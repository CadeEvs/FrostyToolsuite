using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerActionObjectiveEntityData))]
	public class AutoPlayerActionObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerActionObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerActionObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerActionObjectiveEntityData;
		public override string DisplayName => "AutoPlayerActionObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerActionObjectiveEntity(FrostySdk.Ebx.AutoPlayerActionObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

