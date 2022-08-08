using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetPathObjectiveEntityData))]
	public class SetPathObjectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetPathObjectiveEntityData>
	{
		public new FrostySdk.Ebx.SetPathObjectiveEntityData Data => data as FrostySdk.Ebx.SetPathObjectiveEntityData;
		public override string DisplayName => "SetPathObjective";

		public SetPathObjectiveEntity(FrostySdk.Ebx.SetPathObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

