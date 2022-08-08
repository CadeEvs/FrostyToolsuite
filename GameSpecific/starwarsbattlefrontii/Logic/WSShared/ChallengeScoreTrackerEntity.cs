using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengeScoreTrackerEntityData))]
	public class ChallengeScoreTrackerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengeScoreTrackerEntityData>
	{
		public new FrostySdk.Ebx.ChallengeScoreTrackerEntityData Data => data as FrostySdk.Ebx.ChallengeScoreTrackerEntityData;
		public override string DisplayName => "ChallengeScoreTracker";

		public ChallengeScoreTrackerEntity(FrostySdk.Ebx.ChallengeScoreTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

