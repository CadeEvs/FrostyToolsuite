using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILinkTargetURLBuilderEntityData))]
	public class UILinkTargetURLBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILinkTargetURLBuilderEntityData>
	{
		public new FrostySdk.Ebx.UILinkTargetURLBuilderEntityData Data => data as FrostySdk.Ebx.UILinkTargetURLBuilderEntityData;
		public override string DisplayName => "UILinkTargetURLBuilder";

		public UILinkTargetURLBuilderEntity(FrostySdk.Ebx.UILinkTargetURLBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

