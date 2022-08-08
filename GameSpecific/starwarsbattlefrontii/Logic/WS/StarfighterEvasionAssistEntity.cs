using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterEvasionAssistEntityData))]
	public class StarfighterEvasionAssistEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterEvasionAssistEntityData>
	{
		public new FrostySdk.Ebx.StarfighterEvasionAssistEntityData Data => data as FrostySdk.Ebx.StarfighterEvasionAssistEntityData;
		public override string DisplayName => "StarfighterEvasionAssist";

		public StarfighterEvasionAssistEntity(FrostySdk.Ebx.StarfighterEvasionAssistEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

