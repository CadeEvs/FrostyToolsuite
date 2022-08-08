using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LargeParticleCloudEntityData))]
	public class LargeParticleCloudEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.LargeParticleCloudEntityData>
	{
		public new FrostySdk.Ebx.LargeParticleCloudEntityData Data => data as FrostySdk.Ebx.LargeParticleCloudEntityData;

		public LargeParticleCloudEntity(FrostySdk.Ebx.LargeParticleCloudEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

