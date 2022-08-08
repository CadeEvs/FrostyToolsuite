using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveAssassinationEntityData))]
	public class ScoreObjectiveAssassinationEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveAssassinationEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveAssassinationEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveAssassinationEntityData;
		public override string DisplayName => "ScoreObjectiveAssassination";

		public ScoreObjectiveAssassinationEntity(FrostySdk.Ebx.ScoreObjectiveAssassinationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

