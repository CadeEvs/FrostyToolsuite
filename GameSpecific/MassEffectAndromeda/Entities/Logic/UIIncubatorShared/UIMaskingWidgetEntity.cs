using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMaskingWidgetEntityData))]
	public class UIMaskingWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIMaskingWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIMaskingWidgetEntityData Data => data as FrostySdk.Ebx.UIMaskingWidgetEntityData;
		public override string DisplayName => "UIMaskingWidget";

		public UIMaskingWidgetEntity(FrostySdk.Ebx.UIMaskingWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

