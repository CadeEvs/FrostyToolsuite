using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICompassWidgetEntityData))]
	public class UICompassWidgetEntity : UILocationMarkerWidgetEntity, IEntityData<FrostySdk.Ebx.UICompassWidgetEntityData>
	{
		public new FrostySdk.Ebx.UICompassWidgetEntityData Data => data as FrostySdk.Ebx.UICompassWidgetEntityData;
		public override string DisplayName => "UICompassWidget";

		public UICompassWidgetEntity(FrostySdk.Ebx.UICompassWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

