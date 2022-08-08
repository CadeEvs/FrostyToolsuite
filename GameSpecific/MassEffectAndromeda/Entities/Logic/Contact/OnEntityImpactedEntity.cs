using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnEntityImpactedEntityData))]
	public class OnEntityImpactedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnEntityImpactedEntityData>
	{
		public new FrostySdk.Ebx.OnEntityImpactedEntityData Data => data as FrostySdk.Ebx.OnEntityImpactedEntityData;
		public override string DisplayName => "OnEntityImpacted";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnEntityImpacted", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("HitByMelee", Direction.Out)
			};
		}

		public OnEntityImpactedEntity(FrostySdk.Ebx.OnEntityImpactedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

