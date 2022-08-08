using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineLoadoutDataProviderData))]
	public class OnlineLoadoutDataProvider : LoadoutDataProviderBase, IEntityData<FrostySdk.Ebx.OnlineLoadoutDataProviderData>
	{
		public new FrostySdk.Ebx.OnlineLoadoutDataProviderData Data => data as FrostySdk.Ebx.OnlineLoadoutDataProviderData;
		public override string DisplayName => "OnlineLoadoutDataProvider";

		public OnlineLoadoutDataProvider(FrostySdk.Ebx.OnlineLoadoutDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

