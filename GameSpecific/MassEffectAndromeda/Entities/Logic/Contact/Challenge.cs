using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengeData))]
	public class Challenge : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengeData>
	{
		public new FrostySdk.Ebx.ChallengeData Data => data as FrostySdk.Ebx.ChallengeData;
		public override string DisplayName => "Challenge";

		public Challenge(FrostySdk.Ebx.ChallengeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

