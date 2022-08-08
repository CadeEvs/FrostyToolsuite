using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSpawnBundleGroupRequestsStreamingBaseEntityData))]
	public class WSSpawnBundleGroupRequestsStreamingBaseEntity : SpawnBundleGroupRequestsStreamingBaseEntity, IEntityData<FrostySdk.Ebx.WSSpawnBundleGroupRequestsStreamingBaseEntityData>
	{
		public new FrostySdk.Ebx.WSSpawnBundleGroupRequestsStreamingBaseEntityData Data => data as FrostySdk.Ebx.WSSpawnBundleGroupRequestsStreamingBaseEntityData;
		public override string DisplayName => "WSSpawnBundleGroupRequestsStreamingBase";

		public WSSpawnBundleGroupRequestsStreamingBaseEntity(FrostySdk.Ebx.WSSpawnBundleGroupRequestsStreamingBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

