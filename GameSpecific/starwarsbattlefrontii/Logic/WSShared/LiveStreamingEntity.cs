using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LiveStreamingEntityData))]
	public class LiveStreamingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LiveStreamingEntityData>
	{
		public new FrostySdk.Ebx.LiveStreamingEntityData Data => data as FrostySdk.Ebx.LiveStreamingEntityData;
		public override string DisplayName => "LiveStreaming";

		public LiveStreamingEntity(FrostySdk.Ebx.LiveStreamingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

