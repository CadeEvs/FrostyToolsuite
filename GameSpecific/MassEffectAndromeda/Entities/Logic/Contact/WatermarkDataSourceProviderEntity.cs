using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WatermarkDataSourceProviderEntityData))]
	public class WatermarkDataSourceProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WatermarkDataSourceProviderEntityData>
	{
		public new FrostySdk.Ebx.WatermarkDataSourceProviderEntityData Data => data as FrostySdk.Ebx.WatermarkDataSourceProviderEntityData;
		public override string DisplayName => "WatermarkDataSourceProvider";

		public WatermarkDataSourceProviderEntity(FrostySdk.Ebx.WatermarkDataSourceProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

