using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearMediaBillboardProviderEntityData))]
	public class LinearMediaBillboardProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LinearMediaBillboardProviderEntityData>
	{
		public new FrostySdk.Ebx.LinearMediaBillboardProviderEntityData Data => data as FrostySdk.Ebx.LinearMediaBillboardProviderEntityData;
		public override string DisplayName => "LinearMediaBillboardProvider";

		public LinearMediaBillboardProviderEntity(FrostySdk.Ebx.LinearMediaBillboardProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

