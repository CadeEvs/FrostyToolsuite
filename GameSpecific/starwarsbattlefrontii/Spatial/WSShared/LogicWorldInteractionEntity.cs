using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicWorldInteractionEntityData))]
	public class LogicWorldInteractionEntity : WorldInteractionEntity, IEntityData<FrostySdk.Ebx.LogicWorldInteractionEntityData>
	{
		public new FrostySdk.Ebx.LogicWorldInteractionEntityData Data => data as FrostySdk.Ebx.LogicWorldInteractionEntityData;

		public LogicWorldInteractionEntity(FrostySdk.Ebx.LogicWorldInteractionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

