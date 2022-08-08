using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroundHeightEntityData))]
	public class GroundHeightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.GroundHeightEntityData>
	{
		public new FrostySdk.Ebx.GroundHeightEntityData Data => data as FrostySdk.Ebx.GroundHeightEntityData;

		public GroundHeightEntity(FrostySdk.Ebx.GroundHeightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

