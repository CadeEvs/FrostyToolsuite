using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FirstPartyStoreUIData))]
	public class FirstPartyStoreUI : LogicEntity, IEntityData<FrostySdk.Ebx.FirstPartyStoreUIData>
	{
		public new FrostySdk.Ebx.FirstPartyStoreUIData Data => data as FrostySdk.Ebx.FirstPartyStoreUIData;
		public override string DisplayName => "FirstPartyStoreUI";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ProductId", Direction.In)
			};
		}

		public FirstPartyStoreUI(FrostySdk.Ebx.FirstPartyStoreUIData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

