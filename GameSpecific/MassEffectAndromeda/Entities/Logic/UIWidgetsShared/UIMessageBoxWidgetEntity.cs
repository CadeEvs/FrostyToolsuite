using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMessageBoxWidgetEntityData))]
	public class UIMessageBoxWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIMessageBoxWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIMessageBoxWidgetEntityData Data => data as FrostySdk.Ebx.UIMessageBoxWidgetEntityData;
		public override string DisplayName => "UIMessageBoxWidget";

		public UIMessageBoxWidgetEntity(FrostySdk.Ebx.UIMessageBoxWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

