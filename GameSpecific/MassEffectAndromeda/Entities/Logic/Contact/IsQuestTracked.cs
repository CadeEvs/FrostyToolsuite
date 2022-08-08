using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsQuestTrackedData))]
	public class IsQuestTracked : LogicEntity, IEntityData<FrostySdk.Ebx.IsQuestTrackedData>
	{
		public new FrostySdk.Ebx.IsQuestTrackedData Data => data as FrostySdk.Ebx.IsQuestTrackedData;
		public override string DisplayName => "IsQuestTracked";

		public IsQuestTracked(FrostySdk.Ebx.IsQuestTrackedData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

