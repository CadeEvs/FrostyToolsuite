using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineConsumableDataProviderData))]
	public class OnlineConsumableDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineConsumableDataProviderData>
	{
		public new FrostySdk.Ebx.OnlineConsumableDataProviderData Data => data as FrostySdk.Ebx.OnlineConsumableDataProviderData;
		public override string DisplayName => "OnlineConsumableDataProvider";

		public OnlineConsumableDataProvider(FrostySdk.Ebx.OnlineConsumableDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

