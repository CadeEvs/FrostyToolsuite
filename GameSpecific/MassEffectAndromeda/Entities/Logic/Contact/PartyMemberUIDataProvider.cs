using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberUIDataProviderData))]
	public class PartyMemberUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.PartyMemberUIDataProviderData>
	{
		public new FrostySdk.Ebx.PartyMemberUIDataProviderData Data => data as FrostySdk.Ebx.PartyMemberUIDataProviderData;
		public override string DisplayName => "PartyMemberUIDataProvider";

		public PartyMemberUIDataProvider(FrostySdk.Ebx.PartyMemberUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

