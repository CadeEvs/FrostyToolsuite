using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientQuestClaimedInfoEntityData))]
	public class ClientQuestClaimedInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientQuestClaimedInfoEntityData>
	{
		public new FrostySdk.Ebx.ClientQuestClaimedInfoEntityData Data => data as FrostySdk.Ebx.ClientQuestClaimedInfoEntityData;
		public override string DisplayName => "ClientQuestClaimedInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientQuestClaimedInfoEntity(FrostySdk.Ebx.ClientQuestClaimedInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

