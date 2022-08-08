using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILinkHistoryEntityData))]
	public class UILinkHistoryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILinkHistoryEntityData>
	{
		public new FrostySdk.Ebx.UILinkHistoryEntityData Data => data as FrostySdk.Ebx.UILinkHistoryEntityData;
		public override string DisplayName => "UILinkHistory";

		public UILinkHistoryEntity(FrostySdk.Ebx.UILinkHistoryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

