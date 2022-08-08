using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientBadConnectionEntityData))]
	public class ClientBadConnectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientBadConnectionEntityData>
	{
		public new FrostySdk.Ebx.ClientBadConnectionEntityData Data => data as FrostySdk.Ebx.ClientBadConnectionEntityData;
		public override string DisplayName => "ClientBadConnection";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientBadConnectionEntity(FrostySdk.Ebx.ClientBadConnectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

