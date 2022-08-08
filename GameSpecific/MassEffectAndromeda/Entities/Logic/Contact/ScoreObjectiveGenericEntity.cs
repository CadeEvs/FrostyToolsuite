using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveGenericEntityData))]
	public class ScoreObjectiveGenericEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveGenericEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveGenericEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveGenericEntityData;
		public override string DisplayName => "ScoreObjectiveGeneric";

		public ScoreObjectiveGenericEntity(FrostySdk.Ebx.ScoreObjectiveGenericEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

