using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSExplosionPackEntityData))]
	public class WSExplosionPackEntity : ExplosionPackEntity, IEntityData<FrostySdk.Ebx.WSExplosionPackEntityData>
	{
		public new FrostySdk.Ebx.WSExplosionPackEntityData Data => data as FrostySdk.Ebx.WSExplosionPackEntityData;

		public WSExplosionPackEntity(FrostySdk.Ebx.WSExplosionPackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

