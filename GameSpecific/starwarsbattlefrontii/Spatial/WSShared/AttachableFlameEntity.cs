using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachableFlameEntityData))]
	public class AttachableFlameEntity : ProjectileEntity, IEntityData<FrostySdk.Ebx.AttachableFlameEntityData>
	{
		public new FrostySdk.Ebx.AttachableFlameEntityData Data => data as FrostySdk.Ebx.AttachableFlameEntityData;

		public AttachableFlameEntity(FrostySdk.Ebx.AttachableFlameEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

