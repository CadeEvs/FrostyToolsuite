using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BakeableTextureEntityData))]
	public class BakeableTextureEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.BakeableTextureEntityData>
	{
		public new FrostySdk.Ebx.BakeableTextureEntityData Data => data as FrostySdk.Ebx.BakeableTextureEntityData;

		public BakeableTextureEntity(FrostySdk.Ebx.BakeableTextureEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

