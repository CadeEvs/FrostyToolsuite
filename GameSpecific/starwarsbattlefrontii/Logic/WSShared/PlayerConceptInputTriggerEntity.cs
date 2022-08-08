using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerConceptInputTriggerEntityData))]
	public class PlayerConceptInputTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerConceptInputTriggerEntityData>
	{
		public new FrostySdk.Ebx.PlayerConceptInputTriggerEntityData Data => data as FrostySdk.Ebx.PlayerConceptInputTriggerEntityData;
		public override string DisplayName => "PlayerConceptInputTrigger";

		public PlayerConceptInputTriggerEntity(FrostySdk.Ebx.PlayerConceptInputTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

