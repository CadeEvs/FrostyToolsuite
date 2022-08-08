using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILoadingWidgetEntityData))]
	public class UILoadingWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UILoadingWidgetEntityData>
	{
		public new FrostySdk.Ebx.UILoadingWidgetEntityData Data => data as FrostySdk.Ebx.UILoadingWidgetEntityData;
		public override string DisplayName => "UILoadingWidget";

		public UILoadingWidgetEntity(FrostySdk.Ebx.UILoadingWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

