using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreAggregationEntityData))]
	public class ScoreAggregationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreAggregationEntityData>
	{
		public new FrostySdk.Ebx.ScoreAggregationEntityData Data => data as FrostySdk.Ebx.ScoreAggregationEntityData;
		public override string DisplayName => "ScoreAggregation";

		public ScoreAggregationEntity(FrostySdk.Ebx.ScoreAggregationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

