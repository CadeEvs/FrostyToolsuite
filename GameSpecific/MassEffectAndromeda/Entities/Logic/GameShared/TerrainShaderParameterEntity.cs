using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainShaderParameterEntityData))]
	public class TerrainShaderParameterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TerrainShaderParameterEntityData>
	{
		public new FrostySdk.Ebx.TerrainShaderParameterEntityData Data => data as FrostySdk.Ebx.TerrainShaderParameterEntityData;
		public override string DisplayName => "TerrainShaderParameter";

		public TerrainShaderParameterEntity(FrostySdk.Ebx.TerrainShaderParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

