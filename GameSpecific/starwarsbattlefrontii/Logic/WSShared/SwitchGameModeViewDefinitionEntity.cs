using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SwitchGameModeViewDefinitionEntityData))]
	public class SwitchGameModeViewDefinitionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SwitchGameModeViewDefinitionEntityData>
	{
		public new FrostySdk.Ebx.SwitchGameModeViewDefinitionEntityData Data => data as FrostySdk.Ebx.SwitchGameModeViewDefinitionEntityData;
		public override string DisplayName => "SwitchGameModeViewDefinition";

		public SwitchGameModeViewDefinitionEntity(FrostySdk.Ebx.SwitchGameModeViewDefinitionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

