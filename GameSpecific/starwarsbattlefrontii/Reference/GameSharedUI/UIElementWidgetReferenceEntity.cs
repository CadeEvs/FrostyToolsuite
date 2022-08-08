using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementWidgetReferenceEntityData))]
	public class UIElementWidgetReferenceEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.UIElementWidgetReferenceEntityData>
	{
		public new FrostySdk.Ebx.UIElementWidgetReferenceEntityData Data => data as FrostySdk.Ebx.UIElementWidgetReferenceEntityData;

		public UIElementWidgetReferenceEntity(FrostySdk.Ebx.UIElementWidgetReferenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

