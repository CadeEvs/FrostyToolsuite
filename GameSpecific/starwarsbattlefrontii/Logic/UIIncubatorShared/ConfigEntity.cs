using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConfigEntityData))]
	public class ConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConfigEntityData>
	{
		public new FrostySdk.Ebx.ConfigEntityData Data => data as FrostySdk.Ebx.ConfigEntityData;
		public override string DisplayName => "Config";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConfigEntity(FrostySdk.Ebx.ConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

