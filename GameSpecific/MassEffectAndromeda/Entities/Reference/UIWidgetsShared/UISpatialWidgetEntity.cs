using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISpatialWidgetEntityData))]
	public class UISpatialWidgetEntity : UIScreenRenderEntity, IEntityData<FrostySdk.Ebx.UISpatialWidgetEntityData>
	{
		public new FrostySdk.Ebx.UISpatialWidgetEntityData Data => data as FrostySdk.Ebx.UISpatialWidgetEntityData;

		public UISpatialWidgetEntity(FrostySdk.Ebx.UISpatialWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

