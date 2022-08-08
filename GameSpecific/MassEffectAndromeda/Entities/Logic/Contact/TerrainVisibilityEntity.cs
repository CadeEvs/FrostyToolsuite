using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainVisibilityEntityData))]
	public class TerrainVisibilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TerrainVisibilityEntityData>
	{
		public new FrostySdk.Ebx.TerrainVisibilityEntityData Data => data as FrostySdk.Ebx.TerrainVisibilityEntityData;
		public override string DisplayName => "TerrainVisibility";

		public TerrainVisibilityEntity(FrostySdk.Ebx.TerrainVisibilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

