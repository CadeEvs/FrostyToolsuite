using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventCreatorEntityData))]
	public class GameEventCreatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameEventCreatorEntityData>
	{
		public new FrostySdk.Ebx.GameEventCreatorEntityData Data => data as FrostySdk.Ebx.GameEventCreatorEntityData;
		public override string DisplayName => "GameEventCreator";

		public GameEventCreatorEntity(FrostySdk.Ebx.GameEventCreatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

