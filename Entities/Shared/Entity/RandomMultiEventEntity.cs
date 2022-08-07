using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomMultiEventEntityData))]
	public class RandomMultiEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomMultiEventEntityData>
	{
		public new FrostySdk.Ebx.RandomMultiEventEntityData Data => data as FrostySdk.Ebx.RandomMultiEventEntityData;
		public override string DisplayName => "RandomMultiEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.Add(new ConnectionDesc("In", Direction.In));
				for (int i = 0; i < Data.RandomEventWeight.Count; i++)
				{
					outEvents.Add(new ConnectionDesc() { Name = $"Out{i}", Direction = Direction.Out });
				}
				return outEvents;
            }
		}
		public RandomMultiEventEntity(FrostySdk.Ebx.RandomMultiEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

