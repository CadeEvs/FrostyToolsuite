using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicWidgetEntityData))]
	public class DynamicWidgetEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.DynamicWidgetEntityData>
	{
		public new FrostySdk.Ebx.DynamicWidgetEntityData Data => data as FrostySdk.Ebx.DynamicWidgetEntityData;

		public DynamicWidgetEntity(FrostySdk.Ebx.DynamicWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

