using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerObjectiveEntityData))]
	public class AutoPlayerObjectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoPlayerObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerObjectiveEntityData;
		public override string DisplayName => "AutoPlayerObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerObjectiveEntity(FrostySdk.Ebx.AutoPlayerObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

