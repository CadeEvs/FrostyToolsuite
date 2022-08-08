using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DecalEntityData))]
	public class DecalEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.DecalEntityData>
	{
		public new FrostySdk.Ebx.DecalEntityData Data => data as FrostySdk.Ebx.DecalEntityData;

		public DecalEntity(FrostySdk.Ebx.DecalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

