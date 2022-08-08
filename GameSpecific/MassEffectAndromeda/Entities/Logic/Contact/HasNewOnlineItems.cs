using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewOnlineItemsData))]
	public class HasNewOnlineItems : LogicEntity, IEntityData<FrostySdk.Ebx.HasNewOnlineItemsData>
	{
		public new FrostySdk.Ebx.HasNewOnlineItemsData Data => data as FrostySdk.Ebx.HasNewOnlineItemsData;
		public override string DisplayName => "HasNewOnlineItems";

		public HasNewOnlineItems(FrostySdk.Ebx.HasNewOnlineItemsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

