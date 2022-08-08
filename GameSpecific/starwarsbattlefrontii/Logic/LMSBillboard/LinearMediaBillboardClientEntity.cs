using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearMediaBillboardClientEntityData))]
	public class LinearMediaBillboardClientEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LinearMediaBillboardClientEntityData>
	{
		public new FrostySdk.Ebx.LinearMediaBillboardClientEntityData Data => data as FrostySdk.Ebx.LinearMediaBillboardClientEntityData;
		public override string DisplayName => "LinearMediaBillboardClient";

		public LinearMediaBillboardClientEntity(FrostySdk.Ebx.LinearMediaBillboardClientEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

