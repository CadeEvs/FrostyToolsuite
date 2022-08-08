using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExplosionEntityData))]
	public class ExplosionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ExplosionEntityData>
	{
		public new FrostySdk.Ebx.ExplosionEntityData Data => data as FrostySdk.Ebx.ExplosionEntityData;

		public ExplosionEntity(FrostySdk.Ebx.ExplosionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

