using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FireProjectileEntityData))]
	public class FireProjectileEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FireProjectileEntityData>
	{
		public new FrostySdk.Ebx.FireProjectileEntityData Data => data as FrostySdk.Ebx.FireProjectileEntityData;
		public override string DisplayName => "FireProjectile";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FireProjectileEntity(FrostySdk.Ebx.FireProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

