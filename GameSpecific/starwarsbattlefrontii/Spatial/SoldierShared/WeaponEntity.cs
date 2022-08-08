using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponEntityData))]
	public class WeaponEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.WeaponEntityData>
	{
		public new FrostySdk.Ebx.WeaponEntityData Data => data as FrostySdk.Ebx.WeaponEntityData;

		public WeaponEntity(FrostySdk.Ebx.WeaponEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

