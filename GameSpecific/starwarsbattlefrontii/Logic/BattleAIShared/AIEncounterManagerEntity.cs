using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIEncounterManagerEntityData))]
	public class AIEncounterManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIEncounterManagerEntityData>
	{
		public new FrostySdk.Ebx.AIEncounterManagerEntityData Data => data as FrostySdk.Ebx.AIEncounterManagerEntityData;
		public override string DisplayName => "AIEncounterManager";

		public AIEncounterManagerEntity(FrostySdk.Ebx.AIEncounterManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

