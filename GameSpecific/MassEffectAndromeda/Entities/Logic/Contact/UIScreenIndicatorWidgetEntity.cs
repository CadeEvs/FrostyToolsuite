using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenIndicatorWidgetEntityData))]
	public class UIScreenIndicatorWidgetEntity : UILocationMarkerWidgetEntity, IEntityData<FrostySdk.Ebx.UIScreenIndicatorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIScreenIndicatorWidgetEntityData Data => data as FrostySdk.Ebx.UIScreenIndicatorWidgetEntityData;
		public override string DisplayName => "UIScreenIndicatorWidget";

		public UIScreenIndicatorWidgetEntity(FrostySdk.Ebx.UIScreenIndicatorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

