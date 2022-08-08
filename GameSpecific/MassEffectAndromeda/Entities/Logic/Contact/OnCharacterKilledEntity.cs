using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnCharacterKilledEntityData))]
	public class OnCharacterKilledEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnCharacterKilledEntityData>
	{
		public new FrostySdk.Ebx.OnCharacterKilledEntityData Data => data as FrostySdk.Ebx.OnCharacterKilledEntityData;
		public override string DisplayName => "OnCharacterKilled";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnCharacterKilled", Direction.Out)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("KillingPower", Direction.Out),
				new ConnectionDesc("KillingWeapon", Direction.Out),
				new ConnectionDesc("KilledByMelee", Direction.Out)
			};
		}

        public OnCharacterKilledEntity(FrostySdk.Ebx.OnCharacterKilledEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

