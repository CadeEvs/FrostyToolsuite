using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyManagerInfoEntityData))]
	public class PartyManagerInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PartyManagerInfoEntityData>
	{
		public new FrostySdk.Ebx.PartyManagerInfoEntityData Data => data as FrostySdk.Ebx.PartyManagerInfoEntityData;
		public override string DisplayName => "PartyManagerInfo";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("PartyMemberIndex", Direction.In)
			};
		}

		public PartyManagerInfoEntity(FrostySdk.Ebx.PartyManagerInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

