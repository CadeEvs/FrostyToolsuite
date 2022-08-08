using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEImpactTargetEntityData))]
	public class MEImpactTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEImpactTargetEntityData>
	{
		public new FrostySdk.Ebx.MEImpactTargetEntityData Data => data as FrostySdk.Ebx.MEImpactTargetEntityData;
		public override string DisplayName => "MEImpactTarget";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ImpactTarget", Direction.In),
				new ConnectionDesc("ImpactGiver", Direction.In)
			};
		}

		public MEImpactTargetEntity(FrostySdk.Ebx.MEImpactTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

