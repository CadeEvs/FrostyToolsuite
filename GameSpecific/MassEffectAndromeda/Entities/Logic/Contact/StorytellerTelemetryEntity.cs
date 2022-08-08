using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerTelemetryEntityData))]
	public class StorytellerTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StorytellerTelemetryEntityData>
	{
		public new FrostySdk.Ebx.StorytellerTelemetryEntityData Data => data as FrostySdk.Ebx.StorytellerTelemetryEntityData;
		public override string DisplayName => "StorytellerTelemetry";

		public StorytellerTelemetryEntity(FrostySdk.Ebx.StorytellerTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

