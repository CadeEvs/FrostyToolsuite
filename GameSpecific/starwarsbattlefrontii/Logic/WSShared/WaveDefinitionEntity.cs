using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaveDefinitionEntityData))]
	public class WaveDefinitionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WaveDefinitionEntityData>
	{
		public new FrostySdk.Ebx.WaveDefinitionEntityData Data => data as FrostySdk.Ebx.WaveDefinitionEntityData;
		public override string DisplayName => "WaveDefinition";

		public WaveDefinitionEntity(FrostySdk.Ebx.WaveDefinitionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

