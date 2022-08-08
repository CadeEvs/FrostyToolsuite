using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreObjectiveRetrieveEntityData))]
	public class ScoreObjectiveRetrieveEntity : ScoreObjectiveEntity, IEntityData<FrostySdk.Ebx.ScoreObjectiveRetrieveEntityData>
	{
		public new FrostySdk.Ebx.ScoreObjectiveRetrieveEntityData Data => data as FrostySdk.Ebx.ScoreObjectiveRetrieveEntityData;
		public override string DisplayName => "ScoreObjectiveRetrieve";

		public ScoreObjectiveRetrieveEntity(FrostySdk.Ebx.ScoreObjectiveRetrieveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

