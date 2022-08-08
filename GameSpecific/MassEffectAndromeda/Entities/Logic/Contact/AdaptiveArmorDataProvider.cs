using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdaptiveArmorDataProviderData))]
	public class AdaptiveArmorDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.AdaptiveArmorDataProviderData>
	{
		public new FrostySdk.Ebx.AdaptiveArmorDataProviderData Data => data as FrostySdk.Ebx.AdaptiveArmorDataProviderData;
		public override string DisplayName => "AdaptiveArmorDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AdaptiveArmorDataProvider(FrostySdk.Ebx.AdaptiveArmorDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

