using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EndOfRoundDataWidgetData))]
	public class EndOfRoundDataWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.EndOfRoundDataWidgetData>
	{
		public new FrostySdk.Ebx.EndOfRoundDataWidgetData Data => data as FrostySdk.Ebx.EndOfRoundDataWidgetData;
		public override string DisplayName => "EndOfRoundDataWidget";

		public EndOfRoundDataWidget(FrostySdk.Ebx.EndOfRoundDataWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

