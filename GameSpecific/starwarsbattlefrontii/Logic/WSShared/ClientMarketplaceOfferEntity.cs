using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientMarketplaceOfferEntityData))]
	public class ClientMarketplaceOfferEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientMarketplaceOfferEntityData>
	{
		public new FrostySdk.Ebx.ClientMarketplaceOfferEntityData Data => data as FrostySdk.Ebx.ClientMarketplaceOfferEntityData;
		public override string DisplayName => "ClientMarketplaceOffer";

		public ClientMarketplaceOfferEntity(FrostySdk.Ebx.ClientMarketplaceOfferEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

