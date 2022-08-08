using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineWeaponLoadoutDataSourceData))]
	public class OnlineWeaponLoadoutDataSource : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineWeaponLoadoutDataSourceData>
	{
		public new FrostySdk.Ebx.OnlineWeaponLoadoutDataSourceData Data => data as FrostySdk.Ebx.OnlineWeaponLoadoutDataSourceData;
		public override string DisplayName => "OnlineWeaponLoadoutDataSource";

		public OnlineWeaponLoadoutDataSource(FrostySdk.Ebx.OnlineWeaponLoadoutDataSourceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

