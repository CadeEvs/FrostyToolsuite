using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerInputPlaybackEntityData))]
	public class ClientPlayerInputPlaybackEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerInputPlaybackEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerInputPlaybackEntityData Data => data as FrostySdk.Ebx.ClientPlayerInputPlaybackEntityData;
		public override string DisplayName => "ClientPlayerInputPlayback";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerInputPlaybackEntity(FrostySdk.Ebx.ClientPlayerInputPlaybackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

