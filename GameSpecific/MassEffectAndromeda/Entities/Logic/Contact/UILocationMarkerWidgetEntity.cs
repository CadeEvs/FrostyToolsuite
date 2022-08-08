using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILocationMarkerWidgetEntityData))]
	public class UILocationMarkerWidgetEntity : UIChildItemSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UILocationMarkerWidgetEntityData>
	{
		public new FrostySdk.Ebx.UILocationMarkerWidgetEntityData Data => data as FrostySdk.Ebx.UILocationMarkerWidgetEntityData;
		public override string DisplayName => "UILocationMarkerWidget";

		public UILocationMarkerWidgetEntity(FrostySdk.Ebx.UILocationMarkerWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

