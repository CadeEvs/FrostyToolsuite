using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIDamageIndicatorWidgetEntityData))]
	public class UIDamageIndicatorWidgetEntity : UIChildItemSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UIDamageIndicatorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIDamageIndicatorWidgetEntityData Data => data as FrostySdk.Ebx.UIDamageIndicatorWidgetEntityData;
		public override string DisplayName => "UIDamageIndicatorWidget";

		public UIDamageIndicatorWidgetEntity(FrostySdk.Ebx.UIDamageIndicatorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

