using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSBulletCullingControlEntityData))]
	public class WSBulletCullingControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSBulletCullingControlEntityData>
	{
		public new FrostySdk.Ebx.WSBulletCullingControlEntityData Data => data as FrostySdk.Ebx.WSBulletCullingControlEntityData;
		public override string DisplayName => "WSBulletCullingControl";

		public WSBulletCullingControlEntity(FrostySdk.Ebx.WSBulletCullingControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

