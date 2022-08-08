using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WidgetReferenceEntityData))]
	public class WidgetReferenceEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.WidgetReferenceEntityData>
	{
		public new FrostySdk.Ebx.WidgetReferenceEntityData Data => data as FrostySdk.Ebx.WidgetReferenceEntityData;

		public WidgetReferenceEntity(FrostySdk.Ebx.WidgetReferenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

