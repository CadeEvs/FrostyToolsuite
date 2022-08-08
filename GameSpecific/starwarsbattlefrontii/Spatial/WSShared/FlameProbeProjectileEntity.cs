using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlameProbeProjectileEntityData))]
	public class FlameProbeProjectileEntity : FlameProjectileEntity, IEntityData<FrostySdk.Ebx.FlameProbeProjectileEntityData>
	{
		public new FrostySdk.Ebx.FlameProbeProjectileEntityData Data => data as FrostySdk.Ebx.FlameProbeProjectileEntityData;

		public FlameProbeProjectileEntity(FrostySdk.Ebx.FlameProbeProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

