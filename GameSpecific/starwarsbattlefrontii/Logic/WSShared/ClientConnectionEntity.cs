using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientConnectionEntityData))]
	public class ClientConnectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientConnectionEntityData>
	{
		public new FrostySdk.Ebx.ClientConnectionEntityData Data => data as FrostySdk.Ebx.ClientConnectionEntityData;
		public override string DisplayName => "ClientConnection";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientConnectionEntity(FrostySdk.Ebx.ClientConnectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

