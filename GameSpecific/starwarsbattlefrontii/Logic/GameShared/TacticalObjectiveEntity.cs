using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TacticalObjectiveEntityData))]
	public class TacticalObjectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TacticalObjectiveEntityData>
	{
		public new FrostySdk.Ebx.TacticalObjectiveEntityData Data => data as FrostySdk.Ebx.TacticalObjectiveEntityData;
		public override string DisplayName => "TacticalObjective";

		public TacticalObjectiveEntity(FrostySdk.Ebx.TacticalObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

