using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicWidgetCollectorElementData))]
	public class DynamicWidgetCollectorElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.DynamicWidgetCollectorElementData>
	{
		public new FrostySdk.Ebx.DynamicWidgetCollectorElementData Data => data as FrostySdk.Ebx.DynamicWidgetCollectorElementData;
		public override string DisplayName => "DynamicWidgetCollectorElement";

		public DynamicWidgetCollectorElement(FrostySdk.Ebx.DynamicWidgetCollectorElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

