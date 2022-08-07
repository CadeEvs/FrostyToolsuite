using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomEventEntityData))]
	public class RandomEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomEventEntityData>
	{
		public new FrostySdk.Ebx.RandomEventEntityData Data => data as FrostySdk.Ebx.RandomEventEntityData;
		public override string DisplayName => "RandomEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("OnSuccess", Direction.Out)
			};
		}

		public RandomEventEntity(FrostySdk.Ebx.RandomEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

