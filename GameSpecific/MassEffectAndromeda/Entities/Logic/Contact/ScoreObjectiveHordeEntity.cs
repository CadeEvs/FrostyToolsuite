using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveHordeEntityData))]
	public class ScoreObjectiveHordeEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveHordeEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveHordeEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveHordeEntityData;
		public override string DisplayName => "ScoreObjectiveHorde";

		public ScoreObjectiveHordeEntity(FrostySdk.Ebx.ScoreObjectiveHordeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

