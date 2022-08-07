
using LevelEditorPlugin.Controls;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventTrackData))]
	public class EventTrack : SchematicPinTrack, IEntityData<FrostySdk.Ebx.EventTrackData>
	{
		public new FrostySdk.Ebx.EventTrackData Data => data as FrostySdk.Ebx.EventTrackData;
		public override string DisplayName => "EventTrack";
		public override string Icon => "Images/Tracks/EventTrack.png";
        public override IEnumerable<ConnectionDesc> Events
        {
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				if (Data.ExposePins)
				{
					if (Data.TargetPinId != 0)
					{
						outEvents.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(Data.TargetPinId), Direction = Direction.In });
					}
					if (Data.SourcePinId != 0)
					{
						outEvents.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(Data.SourcePinId), Direction = Direction.Out });
					}
				}
				return outEvents;
			}
        }
        public override IEnumerable<ConnectionDesc> Properties => new List<ConnectionDesc>();

		protected float prevTime;
		protected Event<InputEvent> inEvent;
		protected Event<OutputEvent> outEvent;

        public EventTrack(FrostySdk.Ebx.EventTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

        public override void Update(float elapsedTime)
        {
			if (Data.Keyframes.Count > 0)
			{
				for (int i = Data.Keyframes.Count - 1; i >= 0; i--)
				{
					if (prevTime < Data.Keyframes[i].Time && elapsedTime >= Data.Keyframes[i].Time)
					{
						outEvent?.Execute();
					}
				}
			}

			prevTime = elapsedTime;
		}

        public override void InitializeForSchematics(TimelineEntity owningTimeline)
        {
			if (Data.ExposePins)
			{
				if (Data.TargetPinId != 0)
				{
					inEvent = new Event<InputEvent>(owningTimeline, Data.TargetPinId);
				}
				if (Data.SourcePinId != 0)
				{
					outEvent = new Event<OutputEvent>(owningTimeline, Data.SourcePinId);
				}
			}
        }
    }
}

