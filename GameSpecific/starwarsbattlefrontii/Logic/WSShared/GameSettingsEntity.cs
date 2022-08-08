using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameSettingsEntityData))]
	public class GameSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameSettingsEntityData>
	{
		public new FrostySdk.Ebx.GameSettingsEntityData Data => data as FrostySdk.Ebx.GameSettingsEntityData;
		public override string DisplayName => "GameSettings";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameSettingsEntity(FrostySdk.Ebx.GameSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

