using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckGameModeEntityData))]
	public class CheckGameModeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CheckGameModeEntityData>
	{
		public new FrostySdk.Ebx.CheckGameModeEntityData Data => data as FrostySdk.Ebx.CheckGameModeEntityData;
		public override string DisplayName => "CheckGameMode";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CheckGameModeEntity(FrostySdk.Ebx.CheckGameModeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

