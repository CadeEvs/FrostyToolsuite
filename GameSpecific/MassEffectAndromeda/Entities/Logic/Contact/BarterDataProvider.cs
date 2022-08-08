using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BarterDataProviderData))]
	public class BarterDataProvider : MeshSpawner, IEntityData<FrostySdk.Ebx.BarterDataProviderData>
	{
		public new FrostySdk.Ebx.BarterDataProviderData Data => data as FrostySdk.Ebx.BarterDataProviderData;
		public override string DisplayName => "BarterDataProvider";

		public BarterDataProvider(FrostySdk.Ebx.BarterDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

