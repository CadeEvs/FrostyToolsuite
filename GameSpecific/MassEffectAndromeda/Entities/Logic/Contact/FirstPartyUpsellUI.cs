using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FirstPartyUpsellUIData))]
	public class FirstPartyUpsellUI : LogicEntity, IEntityData<FrostySdk.Ebx.FirstPartyUpsellUIData>
	{
		public new FrostySdk.Ebx.FirstPartyUpsellUIData Data => data as FrostySdk.Ebx.FirstPartyUpsellUIData;
		public override string DisplayName => "FirstPartyUpsellUI";

		public FirstPartyUpsellUI(FrostySdk.Ebx.FirstPartyUpsellUIData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

