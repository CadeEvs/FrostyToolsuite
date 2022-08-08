using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEUIButtonLegendWidgetEntityData))]
	public class MEUIButtonLegendWidgetEntity : UIButtonLegendWidgetEntity, IEntityData<FrostySdk.Ebx.MEUIButtonLegendWidgetEntityData>
	{
		public new FrostySdk.Ebx.MEUIButtonLegendWidgetEntityData Data => data as FrostySdk.Ebx.MEUIButtonLegendWidgetEntityData;
		public override string DisplayName => "MEUIButtonLegendWidget";
        public MEUIButtonLegendWidgetEntity(FrostySdk.Ebx.MEUIButtonLegendWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

