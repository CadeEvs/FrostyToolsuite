using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsStreamingBaseEntityData))]
	public class SpawnBundleGroupRequestsStreamingBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsStreamingBaseEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsStreamingBaseEntityData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsStreamingBaseEntityData;
		public override string DisplayName => "SpawnBundleGroupRequestsStreamingBase";

		public SpawnBundleGroupRequestsStreamingBaseEntity(FrostySdk.Ebx.SpawnBundleGroupRequestsStreamingBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

