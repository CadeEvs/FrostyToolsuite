using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleStreamingEntityData))]
	public class SpawnBundleStreamingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnBundleStreamingEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleStreamingEntityData Data => data as FrostySdk.Ebx.SpawnBundleStreamingEntityData;
		public override string DisplayName => "SpawnBundleStreaming";

		public SpawnBundleStreamingEntity(FrostySdk.Ebx.SpawnBundleStreamingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

