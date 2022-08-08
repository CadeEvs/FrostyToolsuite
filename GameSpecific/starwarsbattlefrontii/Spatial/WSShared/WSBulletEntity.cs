using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSBulletEntityData))]
	public class WSBulletEntity : BulletEntity, IEntityData<FrostySdk.Ebx.WSBulletEntityData>
	{
		public new FrostySdk.Ebx.WSBulletEntityData Data => data as FrostySdk.Ebx.WSBulletEntityData;

		public WSBulletEntity(FrostySdk.Ebx.WSBulletEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

