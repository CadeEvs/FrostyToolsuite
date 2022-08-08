using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayTogetherStatusEntityData))]
	public class ClientPlayTogetherStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayTogetherStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayTogetherStatusEntityData Data => data as FrostySdk.Ebx.ClientPlayTogetherStatusEntityData;
		public override string DisplayName => "ClientPlayTogetherStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayTogetherStatusEntity(FrostySdk.Ebx.ClientPlayTogetherStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

