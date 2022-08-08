using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierHealthEntityData))]
	public class MESoldierHealthEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MESoldierHealthEntityData>
	{
		public new FrostySdk.Ebx.MESoldierHealthEntityData Data => data as FrostySdk.Ebx.MESoldierHealthEntityData;
		public override string DisplayName => "MESoldierHealth";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Character", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("HealthValue", Direction.In),
				new ConnectionDesc("ShieldPercentage", Direction.In),
				new ConnectionDesc("CurrentHealth", Direction.Out),
				new ConnectionDesc("MaxHealth", Direction.Out)
			};
		}

		public MESoldierHealthEntity(FrostySdk.Ebx.MESoldierHealthEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

