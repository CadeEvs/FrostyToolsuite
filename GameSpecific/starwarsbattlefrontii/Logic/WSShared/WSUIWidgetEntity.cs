using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIWidgetEntityData))]
	public class WSUIWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.WSUIWidgetEntityData>
	{
		public new FrostySdk.Ebx.WSUIWidgetEntityData Data => data as FrostySdk.Ebx.WSUIWidgetEntityData;
		public override string DisplayName => "WSUIWidget";

		public WSUIWidgetEntity(FrostySdk.Ebx.WSUIWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

