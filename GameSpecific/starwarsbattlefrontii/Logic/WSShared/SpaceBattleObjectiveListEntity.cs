using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceBattleObjectiveListEntityData))]
	public class SpaceBattleObjectiveListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpaceBattleObjectiveListEntityData>
	{
		public new FrostySdk.Ebx.SpaceBattleObjectiveListEntityData Data => data as FrostySdk.Ebx.SpaceBattleObjectiveListEntityData;
		public override string DisplayName => "SpaceBattleObjectiveList";

		public SpaceBattleObjectiveListEntity(FrostySdk.Ebx.SpaceBattleObjectiveListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

