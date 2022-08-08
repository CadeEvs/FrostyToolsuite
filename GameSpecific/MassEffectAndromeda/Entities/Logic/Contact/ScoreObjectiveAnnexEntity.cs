using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveAnnexEntityData))]
	public class ScoreObjectiveAnnexEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveAnnexEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveAnnexEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveAnnexEntityData;
		public override string DisplayName => "ScoreObjectiveAnnex";

		public ScoreObjectiveAnnexEntity(FrostySdk.Ebx.ScoreObjectiveAnnexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

