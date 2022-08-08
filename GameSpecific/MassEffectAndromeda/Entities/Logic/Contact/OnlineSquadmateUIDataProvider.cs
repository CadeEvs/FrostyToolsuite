using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineSquadmateUIDataProviderData))]
	public class OnlineSquadmateUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineSquadmateUIDataProviderData>
	{
		public new FrostySdk.Ebx.OnlineSquadmateUIDataProviderData Data => data as FrostySdk.Ebx.OnlineSquadmateUIDataProviderData;
		public override string DisplayName => "OnlineSquadmateUIDataProvider";

		public OnlineSquadmateUIDataProvider(FrostySdk.Ebx.OnlineSquadmateUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

