using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoosterDataProviderData))]
	public class BoosterDataProvider : OnlineLoadoutDataProvider, IEntityData<FrostySdk.Ebx.BoosterDataProviderData>
	{
		public new FrostySdk.Ebx.BoosterDataProviderData Data => data as FrostySdk.Ebx.BoosterDataProviderData;
		public override string DisplayName => "BoosterDataProvider";

		public BoosterDataProvider(FrostySdk.Ebx.BoosterDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

