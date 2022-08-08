using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerfOverlayDataSourceProviderEntityData))]
	public class PerfOverlayDataSourceProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PerfOverlayDataSourceProviderEntityData>
	{
		public new FrostySdk.Ebx.PerfOverlayDataSourceProviderEntityData Data => data as FrostySdk.Ebx.PerfOverlayDataSourceProviderEntityData;
		public override string DisplayName => "PerfOverlayDataSourceProvider";

		public PerfOverlayDataSourceProviderEntity(FrostySdk.Ebx.PerfOverlayDataSourceProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

