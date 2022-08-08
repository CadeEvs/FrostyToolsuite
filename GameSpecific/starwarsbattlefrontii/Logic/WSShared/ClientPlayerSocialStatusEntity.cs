using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerSocialStatusEntityData))]
	public class ClientPlayerSocialStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerSocialStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerSocialStatusEntityData Data => data as FrostySdk.Ebx.ClientPlayerSocialStatusEntityData;
		public override string DisplayName => "ClientPlayerSocialStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerSocialStatusEntity(FrostySdk.Ebx.ClientPlayerSocialStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

