using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameLoopStateWriterEntityData))]
	public class ClientGameLoopStateWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameLoopStateWriterEntityData>
	{
		public new FrostySdk.Ebx.ClientGameLoopStateWriterEntityData Data => data as FrostySdk.Ebx.ClientGameLoopStateWriterEntityData;
		public override string DisplayName => "ClientGameLoopStateWriter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameLoopStateWriterEntity(FrostySdk.Ebx.ClientGameLoopStateWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

