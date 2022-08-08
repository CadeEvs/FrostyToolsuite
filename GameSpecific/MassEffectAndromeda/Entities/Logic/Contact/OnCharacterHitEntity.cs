using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnCharacterHitEntityData))]
	public class OnCharacterHitEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnCharacterHitEntityData>
	{
		public new FrostySdk.Ebx.OnCharacterHitEntityData Data => data as FrostySdk.Ebx.OnCharacterHitEntityData;
		public override string DisplayName => "OnCharacterHit";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Attacker", Direction.Out),
				new ConnectionDesc("Victim", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnCharacterHit", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("HittingPower", Direction.Out),
				new ConnectionDesc("HittingWeapon", Direction.Out),
				new ConnectionDesc("HitByMelee", Direction.Out),
				new ConnectionDesc("ShieldBreak", Direction.Out),
				new ConnectionDesc("ShieldDamage", Direction.Out),
				new ConnectionDesc("Damage", Direction.Out),
				new ConnectionDesc("HealthBefore", Direction.Out),
				new ConnectionDesc("HealthAfter", Direction.Out)
			};
		}

		public OnCharacterHitEntity(FrostySdk.Ebx.OnCharacterHitEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

