using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveDefuseEntityData))]
	public class ScoreObjectiveDefuseEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveDefuseEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveDefuseEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveDefuseEntityData;
		public override string DisplayName => "ScoreObjectiveDefuse";

		public ScoreObjectiveDefuseEntity(FrostySdk.Ebx.ScoreObjectiveDefuseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

