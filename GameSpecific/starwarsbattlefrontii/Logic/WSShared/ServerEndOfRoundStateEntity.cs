using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerEndOfRoundStateEntityData))]
	public class ServerEndOfRoundStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerEndOfRoundStateEntityData>
	{
		public new FrostySdk.Ebx.ServerEndOfRoundStateEntityData Data => data as FrostySdk.Ebx.ServerEndOfRoundStateEntityData;
		public override string DisplayName => "ServerEndOfRoundState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ServerEndOfRoundStateEntity(FrostySdk.Ebx.ServerEndOfRoundStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

