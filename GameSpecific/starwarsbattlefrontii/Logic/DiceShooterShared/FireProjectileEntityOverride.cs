using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FireProjectileEntityOverrideData))]
	public class FireProjectileEntityOverride : LogicEntity, IEntityData<FrostySdk.Ebx.FireProjectileEntityOverrideData>
	{
		public new FrostySdk.Ebx.FireProjectileEntityOverrideData Data => data as FrostySdk.Ebx.FireProjectileEntityOverrideData;
		public override string DisplayName => "FireProjectileEntityOverride";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FireProjectileEntityOverride(FrostySdk.Ebx.FireProjectileEntityOverrideData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

