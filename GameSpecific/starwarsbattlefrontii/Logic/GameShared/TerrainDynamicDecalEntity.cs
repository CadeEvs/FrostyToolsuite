using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainDynamicDecalEntityData))]
	public class TerrainDynamicDecalEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TerrainDynamicDecalEntityData>
	{
		public new FrostySdk.Ebx.TerrainDynamicDecalEntityData Data => data as FrostySdk.Ebx.TerrainDynamicDecalEntityData;
		public override string DisplayName => "TerrainDynamicDecal";

		public TerrainDynamicDecalEntity(FrostySdk.Ebx.TerrainDynamicDecalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

