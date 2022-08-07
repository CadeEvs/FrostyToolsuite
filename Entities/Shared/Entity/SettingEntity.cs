using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SettingEntityData))]
	public class SettingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SettingEntityData>
	{
		public new FrostySdk.Ebx.SettingEntityData Data => data as FrostySdk.Ebx.SettingEntityData;
		public override string DisplayName => "Setting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SettingEntity(FrostySdk.Ebx.SettingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

