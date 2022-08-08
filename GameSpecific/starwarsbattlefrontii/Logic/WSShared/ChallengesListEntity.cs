using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengesListEntityData))]
	public class ChallengesListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengesListEntityData>
	{
		public new FrostySdk.Ebx.ChallengesListEntityData Data => data as FrostySdk.Ebx.ChallengesListEntityData;
		public override string DisplayName => "ChallengesList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ChallengesListEntity(FrostySdk.Ebx.ChallengesListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

