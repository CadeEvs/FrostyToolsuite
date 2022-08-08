using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DecalRibbonEntityData))]
	public class DecalRibbonEntity : RenderVolumeEntity, IEntityData<FrostySdk.Ebx.DecalRibbonEntityData>
	{
		public new FrostySdk.Ebx.DecalRibbonEntityData Data => data as FrostySdk.Ebx.DecalRibbonEntityData;

		public DecalRibbonEntity(FrostySdk.Ebx.DecalRibbonEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

