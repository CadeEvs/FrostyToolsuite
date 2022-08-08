using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPromotionInfoEntityData))]
	public class ClientPromotionInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPromotionInfoEntityData>
	{
		public new FrostySdk.Ebx.ClientPromotionInfoEntityData Data => data as FrostySdk.Ebx.ClientPromotionInfoEntityData;
		public override string DisplayName => "ClientPromotionInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPromotionInfoEntity(FrostySdk.Ebx.ClientPromotionInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

