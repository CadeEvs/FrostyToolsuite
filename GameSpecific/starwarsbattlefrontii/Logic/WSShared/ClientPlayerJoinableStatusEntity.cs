using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerJoinableStatusEntityData))]
	public class ClientPlayerJoinableStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerJoinableStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerJoinableStatusEntityData Data => data as FrostySdk.Ebx.ClientPlayerJoinableStatusEntityData;
		public override string DisplayName => "ClientPlayerJoinableStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerJoinableStatusEntity(FrostySdk.Ebx.ClientPlayerJoinableStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

