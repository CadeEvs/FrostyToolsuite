using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreditsListWidgetData))]
	public class CreditsListWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.CreditsListWidgetData>
	{
		public new FrostySdk.Ebx.CreditsListWidgetData Data => data as FrostySdk.Ebx.CreditsListWidgetData;
		public override string DisplayName => "CreditsListWidget";

		public CreditsListWidget(FrostySdk.Ebx.CreditsListWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

