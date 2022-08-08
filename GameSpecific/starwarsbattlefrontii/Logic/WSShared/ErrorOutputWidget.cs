using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ErrorOutputWidgetData))]
	public class ErrorOutputWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.ErrorOutputWidgetData>
	{
		public new FrostySdk.Ebx.ErrorOutputWidgetData Data => data as FrostySdk.Ebx.ErrorOutputWidgetData;
		public override string DisplayName => "ErrorOutputWidget";

		public ErrorOutputWidget(FrostySdk.Ebx.ErrorOutputWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

