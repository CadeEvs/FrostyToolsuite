using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventQueueEntityData))]
	public class EventQueueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventQueueEntityData>
	{
		public new FrostySdk.Ebx.EventQueueEntityData Data => data as FrostySdk.Ebx.EventQueueEntityData;
		public override string DisplayName => "EventQueue";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.Add(new ConnectionDesc("In", Direction.In));

                foreach (var eventQueueItem in Data.EventQueue)
                {
                    outEvents.Add(new ConnectionDesc() { Name = eventQueueItem, Direction = Direction.Out });
                }

                return outEvents;
            }
        }

        public EventQueueEntity(FrostySdk.Ebx.EventQueueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

