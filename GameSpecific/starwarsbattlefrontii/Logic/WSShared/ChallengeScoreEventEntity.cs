using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengeScoreEventEntityData))]
	public class ChallengeScoreEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengeScoreEventEntityData>
	{
		public new FrostySdk.Ebx.ChallengeScoreEventEntityData Data => data as FrostySdk.Ebx.ChallengeScoreEventEntityData;
		public override string DisplayName => "ChallengeScoreEvent";

		public ChallengeScoreEventEntity(FrostySdk.Ebx.ChallengeScoreEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

