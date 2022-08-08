using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterStickyTargetAssistEntityData))]
	public class StarfighterStickyTargetAssistEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterStickyTargetAssistEntityData>
	{
		public new FrostySdk.Ebx.StarfighterStickyTargetAssistEntityData Data => data as FrostySdk.Ebx.StarfighterStickyTargetAssistEntityData;
		public override string DisplayName => "StarfighterStickyTargetAssist";

		public StarfighterStickyTargetAssistEntity(FrostySdk.Ebx.StarfighterStickyTargetAssistEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

