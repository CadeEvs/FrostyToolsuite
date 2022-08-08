using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotSubLevelStreamingTriggerEntityData))]
	public class PlotSubLevelStreamingTriggerEntity : SubLevelStreamingTriggerEntity, IEntityData<FrostySdk.Ebx.PlotSubLevelStreamingTriggerEntityData>
	{
		public new FrostySdk.Ebx.PlotSubLevelStreamingTriggerEntityData Data => data as FrostySdk.Ebx.PlotSubLevelStreamingTriggerEntityData;

		public PlotSubLevelStreamingTriggerEntity(FrostySdk.Ebx.PlotSubLevelStreamingTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

