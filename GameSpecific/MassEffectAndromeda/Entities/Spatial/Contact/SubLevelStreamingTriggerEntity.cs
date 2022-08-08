using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelStreamingTriggerEntityData))]
	public class SubLevelStreamingTriggerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SubLevelStreamingTriggerEntityData>
	{
		public new FrostySdk.Ebx.SubLevelStreamingTriggerEntityData Data => data as FrostySdk.Ebx.SubLevelStreamingTriggerEntityData;

		public SubLevelStreamingTriggerEntity(FrostySdk.Ebx.SubLevelStreamingTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

