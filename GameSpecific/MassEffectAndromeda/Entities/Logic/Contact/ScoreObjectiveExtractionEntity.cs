using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveExtractionEntityData))]
	public class ScoreObjectiveExtractionEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveExtractionEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveExtractionEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveExtractionEntityData;
		public override string DisplayName => "ScoreObjectiveExtraction";

		public ScoreObjectiveExtractionEntity(FrostySdk.Ebx.ScoreObjectiveExtractionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

