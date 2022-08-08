using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LobbyDataProviderData))]
	public class LobbyDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.LobbyDataProviderData>
	{
		public new FrostySdk.Ebx.LobbyDataProviderData Data => data as FrostySdk.Ebx.LobbyDataProviderData;
		public override string DisplayName => "LobbyDataProvider";

		public LobbyDataProvider(FrostySdk.Ebx.LobbyDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

