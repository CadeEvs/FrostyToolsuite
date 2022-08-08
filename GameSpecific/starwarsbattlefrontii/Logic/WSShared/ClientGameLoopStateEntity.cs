using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameLoopStateEntityData))]
	public class ClientGameLoopStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameLoopStateEntityData>
	{
		public new FrostySdk.Ebx.ClientGameLoopStateEntityData Data => data as FrostySdk.Ebx.ClientGameLoopStateEntityData;
		public override string DisplayName => "ClientGameLoopState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameLoopStateEntity(FrostySdk.Ebx.ClientGameLoopStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

