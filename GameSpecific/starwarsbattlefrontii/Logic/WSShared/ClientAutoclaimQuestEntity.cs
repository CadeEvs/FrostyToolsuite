using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientAutoclaimQuestEntityData))]
	public class ClientAutoclaimQuestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientAutoclaimQuestEntityData>
	{
		public new FrostySdk.Ebx.ClientAutoclaimQuestEntityData Data => data as FrostySdk.Ebx.ClientAutoclaimQuestEntityData;
		public override string DisplayName => "ClientAutoclaimQuest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientAutoclaimQuestEntity(FrostySdk.Ebx.ClientAutoclaimQuestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

