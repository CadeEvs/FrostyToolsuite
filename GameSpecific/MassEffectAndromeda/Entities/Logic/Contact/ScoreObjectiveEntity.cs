using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveEntityData))]
	public class ScoreObjectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveEntityData;
		public override string DisplayName => "ScoreObjective";

		public ScoreObjectiveEntity(FrostySdk.Ebx.ScoreObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

