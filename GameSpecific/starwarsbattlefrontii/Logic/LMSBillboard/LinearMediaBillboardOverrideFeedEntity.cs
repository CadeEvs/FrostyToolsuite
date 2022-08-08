using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearMediaBillboardOverrideFeedEntityData))]
	public class LinearMediaBillboardOverrideFeedEntity : LinearMediaBillboardFeedEntity, IEntityData<FrostySdk.Ebx.LinearMediaBillboardOverrideFeedEntityData>
	{
		public new FrostySdk.Ebx.LinearMediaBillboardOverrideFeedEntityData Data => data as FrostySdk.Ebx.LinearMediaBillboardOverrideFeedEntityData;
		public override string DisplayName => "LinearMediaBillboardOverrideFeed";

		public LinearMediaBillboardOverrideFeedEntity(FrostySdk.Ebx.LinearMediaBillboardOverrideFeedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

