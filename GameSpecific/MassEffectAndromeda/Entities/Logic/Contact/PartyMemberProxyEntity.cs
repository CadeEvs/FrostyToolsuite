using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberProxyEntityData))]
	public class PartyMemberProxyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PartyMemberProxyEntityData>
	{
		public new FrostySdk.Ebx.PartyMemberProxyEntityData Data => data as FrostySdk.Ebx.PartyMemberProxyEntityData;
		public override string DisplayName => "PartyMemberProxy";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("PartyMemberLeader", Direction.Out),
				new ConnectionDesc("PartyMember1", Direction.Out),
				new ConnectionDesc("PartyMember2", Direction.Out)
			};
		}
        public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("IsMandatory", Direction.In)
			};
		}

		public PartyMemberProxyEntity(FrostySdk.Ebx.PartyMemberProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

