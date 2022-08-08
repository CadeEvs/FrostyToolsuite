using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponWorldTransformEntityData))]
	public class WeaponWorldTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponWorldTransformEntityData>
	{
		public new FrostySdk.Ebx.WeaponWorldTransformEntityData Data => data as FrostySdk.Ebx.WeaponWorldTransformEntityData;
		public override string DisplayName => "WeaponWorldTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WeaponWorldTransformEntity(FrostySdk.Ebx.WeaponWorldTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

