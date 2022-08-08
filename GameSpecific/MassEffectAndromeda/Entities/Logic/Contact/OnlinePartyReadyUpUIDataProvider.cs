using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlinePartyReadyUpUIDataProviderData))]
	public class OnlinePartyReadyUpUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OnlinePartyReadyUpUIDataProviderData>
	{
		public new FrostySdk.Ebx.OnlinePartyReadyUpUIDataProviderData Data => data as FrostySdk.Ebx.OnlinePartyReadyUpUIDataProviderData;
		public override string DisplayName => "OnlinePartyReadyUpUIDataProvider";

		public OnlinePartyReadyUpUIDataProvider(FrostySdk.Ebx.OnlinePartyReadyUpUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

