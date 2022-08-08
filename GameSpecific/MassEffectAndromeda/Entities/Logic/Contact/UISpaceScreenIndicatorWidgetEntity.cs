using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISpaceScreenIndicatorWidgetEntityData))]
	public class UISpaceScreenIndicatorWidgetEntity : UIScreenIndicatorWidgetEntity, IEntityData<FrostySdk.Ebx.UISpaceScreenIndicatorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UISpaceScreenIndicatorWidgetEntityData Data => data as FrostySdk.Ebx.UISpaceScreenIndicatorWidgetEntityData;
		public override string DisplayName => "UISpaceScreenIndicatorWidget";

		public UISpaceScreenIndicatorWidgetEntity(FrostySdk.Ebx.UISpaceScreenIndicatorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

