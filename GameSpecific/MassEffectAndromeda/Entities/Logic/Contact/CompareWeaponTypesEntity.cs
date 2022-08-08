using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareWeaponTypesEntityData))]
	public class CompareWeaponTypesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CompareWeaponTypesEntityData>
	{
		public new FrostySdk.Ebx.CompareWeaponTypesEntityData Data => data as FrostySdk.Ebx.CompareWeaponTypesEntityData;
		public override string DisplayName => $"CompareWeaponTypes";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("CompareWeaponTypes", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Weapon", Direction.In)
			};
		}
		public override IEnumerable<string> HeaderRows
		{
			get => new List<string>()
			{
				Data.WeaponType.ToString().Replace("MEWeaponType_", "")
			};
		}

		public CompareWeaponTypesEntity(FrostySdk.Ebx.CompareWeaponTypesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

