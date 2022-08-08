using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetEntityData))]
	public class UIWidgetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetEntityData;
		public override string DisplayName => "UIWidget";

		public UIWidgetEntity(FrostySdk.Ebx.UIWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

