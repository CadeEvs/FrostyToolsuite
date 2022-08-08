using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterStickyTargetAssistConfigEntityData))]
	public class StarfighterStickyTargetAssistConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterStickyTargetAssistConfigEntityData>
	{
		public new FrostySdk.Ebx.StarfighterStickyTargetAssistConfigEntityData Data => data as FrostySdk.Ebx.StarfighterStickyTargetAssistConfigEntityData;
		public override string DisplayName => "StarfighterStickyTargetAssistConfig";

		public StarfighterStickyTargetAssistConfigEntity(FrostySdk.Ebx.StarfighterStickyTargetAssistConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

