using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainMaterialContactEntityData))]
	public class TerrainMaterialContactEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TerrainMaterialContactEntityData>
	{
		public new FrostySdk.Ebx.TerrainMaterialContactEntityData Data => data as FrostySdk.Ebx.TerrainMaterialContactEntityData;
		public override string DisplayName => "TerrainMaterialContact";

		public TerrainMaterialContactEntity(FrostySdk.Ebx.TerrainMaterialContactEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

