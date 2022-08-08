using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VOIPVisualFeedbackEnabledEntityData))]
	public class VOIPVisualFeedbackEnabledEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VOIPVisualFeedbackEnabledEntityData>
	{
		public new FrostySdk.Ebx.VOIPVisualFeedbackEnabledEntityData Data => data as FrostySdk.Ebx.VOIPVisualFeedbackEnabledEntityData;
		public override string DisplayName => "VOIPVisualFeedbackEnabled";

		public VOIPVisualFeedbackEnabledEntity(FrostySdk.Ebx.VOIPVisualFeedbackEnabledEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

