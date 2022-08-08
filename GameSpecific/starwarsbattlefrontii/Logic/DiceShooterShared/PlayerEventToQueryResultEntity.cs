using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerEventToQueryResultEntityData))]
	public class PlayerEventToQueryResultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerEventToQueryResultEntityData>
	{
		public new FrostySdk.Ebx.PlayerEventToQueryResultEntityData Data => data as FrostySdk.Ebx.PlayerEventToQueryResultEntityData;
		public override string DisplayName => "PlayerEventToQueryResult";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerEventToQueryResultEntity(FrostySdk.Ebx.PlayerEventToQueryResultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

