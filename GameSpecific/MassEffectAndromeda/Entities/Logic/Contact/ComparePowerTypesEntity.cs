using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ComparePowerTypesEntityData))]
	public class ComparePowerTypesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ComparePowerTypesEntityData>
	{
		public new FrostySdk.Ebx.ComparePowerTypesEntityData Data => data as FrostySdk.Ebx.ComparePowerTypesEntityData;
		public override string DisplayName => "ComparePowerTypes";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ComparePowerTypes", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Power", Direction.In)
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				Data.PowerType.ToString().Replace("MEPowerType_", "")
			};
        }

		public ComparePowerTypesEntity(FrostySdk.Ebx.ComparePowerTypesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

