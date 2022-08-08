using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GrenadeEntityData))]
	public class GrenadeEntity : GhostedProjectileEntity, IEntityData<FrostySdk.Ebx.GrenadeEntityData>
	{
		public new FrostySdk.Ebx.GrenadeEntityData Data => data as FrostySdk.Ebx.GrenadeEntityData;

		public GrenadeEntity(FrostySdk.Ebx.GrenadeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

