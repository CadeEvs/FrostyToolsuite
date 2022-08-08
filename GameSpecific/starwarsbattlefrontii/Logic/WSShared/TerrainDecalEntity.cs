using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainDecalEntityData))]
	public class TerrainDecalEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TerrainDecalEntityData>
	{
		public new FrostySdk.Ebx.TerrainDecalEntityData Data => data as FrostySdk.Ebx.TerrainDecalEntityData;
		public override string DisplayName => "TerrainDecal";

		public TerrainDecalEntity(FrostySdk.Ebx.TerrainDecalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

