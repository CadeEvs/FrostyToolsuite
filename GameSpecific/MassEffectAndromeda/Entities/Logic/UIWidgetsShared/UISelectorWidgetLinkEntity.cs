using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISelectorWidgetLinkEntityData))]
	public class UISelectorWidgetLinkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UISelectorWidgetLinkEntityData>
	{
		public new FrostySdk.Ebx.UISelectorWidgetLinkEntityData Data => data as FrostySdk.Ebx.UISelectorWidgetLinkEntityData;
		public override string DisplayName => "UISelectorWidgetLink";

		public UISelectorWidgetLinkEntity(FrostySdk.Ebx.UISelectorWidgetLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

