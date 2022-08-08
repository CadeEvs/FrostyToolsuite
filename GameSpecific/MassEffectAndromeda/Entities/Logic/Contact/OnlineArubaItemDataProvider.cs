using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineArubaItemDataProviderData))]
	public class OnlineArubaItemDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineArubaItemDataProviderData>
	{
		public new FrostySdk.Ebx.OnlineArubaItemDataProviderData Data => data as FrostySdk.Ebx.OnlineArubaItemDataProviderData;
		public override string DisplayName => "OnlineArubaItemDataProvider";

		public OnlineArubaItemDataProvider(FrostySdk.Ebx.OnlineArubaItemDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

