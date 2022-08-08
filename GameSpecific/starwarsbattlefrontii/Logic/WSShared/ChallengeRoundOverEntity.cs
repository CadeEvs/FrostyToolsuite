using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengeRoundOverEntityData))]
	public class ChallengeRoundOverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengeRoundOverEntityData>
	{
		public new FrostySdk.Ebx.ChallengeRoundOverEntityData Data => data as FrostySdk.Ebx.ChallengeRoundOverEntityData;
		public override string DisplayName => "ChallengeRoundOver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ChallengeRoundOverEntity(FrostySdk.Ebx.ChallengeRoundOverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

