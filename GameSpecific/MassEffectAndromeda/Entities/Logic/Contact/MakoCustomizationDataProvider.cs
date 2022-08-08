using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MakoCustomizationDataProviderData))]
	public class MakoCustomizationDataProvider : MeshSpawner, IEntityData<FrostySdk.Ebx.MakoCustomizationDataProviderData>
	{
		public new FrostySdk.Ebx.MakoCustomizationDataProviderData Data => data as FrostySdk.Ebx.MakoCustomizationDataProviderData;
		public override string DisplayName => "MakoCustomizationDataProvider";

		public MakoCustomizationDataProvider(FrostySdk.Ebx.MakoCustomizationDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

