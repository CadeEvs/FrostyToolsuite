using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearMediaBillboardFeedEntityData))]
	public class LinearMediaBillboardFeedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LinearMediaBillboardFeedEntityData>
	{
		public new FrostySdk.Ebx.LinearMediaBillboardFeedEntityData Data => data as FrostySdk.Ebx.LinearMediaBillboardFeedEntityData;
		public override string DisplayName => "LinearMediaBillboardFeed";

		public LinearMediaBillboardFeedEntity(FrostySdk.Ebx.LinearMediaBillboardFeedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

