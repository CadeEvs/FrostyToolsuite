using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrostySdk.Ebx;
using LinearTransform = FrostySdk.Ebx.LinearTransform;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SequenceEntityData))]
	public class SequenceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SequenceEntityData>
	{
		protected readonly int Event_Start = Frosty.Hash.Fnv1.HashString("Start");
		protected readonly int Event_Stop = Frosty.Hash.Fnv1.HashString("Stop");
		protected readonly int Event_OnFinished = Frosty.Hash.Fnv1.HashString("OnFinished");

		public new FrostySdk.Ebx.SequenceEntityData Data => data as FrostySdk.Ebx.SequenceEntityData;
		public override string DisplayName => "Sequence";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.Add(new ConnectionDesc("Start", Direction.In));
				outEvents.Add(new ConnectionDesc("Stop", Direction.In));
				outEvents.Add(new ConnectionDesc("OnFinished", Direction.Out));
				foreach (SequenceEventData sequenceEvent in Data.Events)
				{
					string name = sequenceEvent.Event.Name;
					if (outEvents.Find(e => e.Name.Equals(name)).Name != name)
                    {
						outEvents.Add(new ConnectionDesc(name, Direction.Out));
                    }
				}
				return outEvents;
			}
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				foreach (PointerRef trackRef in Data.PropertyTracks)
				{
					PropertyTrackData track = trackRef.GetObjectAs<FrostySdk.Ebx.PropertyTrackData>();
					if (track != null)
					{
						outProperties.Add(new ConnectionDesc(FrostySdk.Utils.GetString(track.Id), Direction.Out));
					}
				}
				return outProperties;
            }
        }

        protected Dictionary<int, IProperty> outputProperties = new Dictionary<int, IProperty>();
		protected Dictionary<int, IEvent> outputEvents = new Dictionary<int, IEvent>();

		protected Event<InputEvent> startEvent;
		protected Event<InputEvent> stopEvent;
		protected Event<OutputEvent> onFinishedEvent;

		protected bool sequenceStarted;
		protected Stopwatch timer;
		protected long prevTime;

		public SequenceEntity(FrostySdk.Ebx.SequenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			startEvent = new Event<InputEvent>(this, Event_Start);
			stopEvent = new Event<InputEvent>(this, Event_Stop);
			onFinishedEvent = new Event<OutputEvent>(this, Event_OnFinished);

			OnDataModified();

			timer = new Stopwatch();
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == Event_Start)
			{
				sequenceStarted = true;
				timer.Start();
				return;
			}
			else if (eventHash == Event_Stop)
			{
				sequenceStarted = false;
				timer.Stop();
				return;
			}

            base.OnEvent(eventHash);
        }

        public override void Update_PostFrame()
        {
			if (Data.AutoStart && !sequenceStarted)
			{
				sequenceStarted = true;
				timer.Start();
			}

			if (sequenceStarted)
			{
				long elapsedTime = timer.ElapsedMilliseconds;
				foreach (PointerRef trackRef in Data.PropertyTracks)
				{
					PropertyTrackData track = trackRef.GetObjectAs<FrostySdk.Ebx.PropertyTrackData>();
					int index = -1;

					for (int i = 0; i < track.Times.Count; i++)
					{
						if (elapsedTime >= track.Times[i])
						{
							index = i;
						}
					}

					if (track is FrostySdk.Ebx.FloatPropertyTrackData)
					{
						FloatPropertyTrackData floatTrack = track as FrostySdk.Ebx.FloatPropertyTrackData;
						float currentValue = 0.0f;

						if (index + 1 >= track.Times.Count)
						{
							currentValue = floatTrack.Values[index];

							if (Data.Looping)
							{
								timer.Restart();
							}
							else
							{
								sequenceStarted = false;
								timer.Reset();

								CallEvent(Event_OnFinished);
							}
						}
						else if (index == -1)
						{
							currentValue = floatTrack.Values[0];
						}
						else
						{
							float prevValue = floatTrack.Values[index];
							float nextValue = floatTrack.Values[index + 1];
							float prevTime = track.Times[index];
							float nextTime = track.Times[index + 1];

							currentValue = SharpDX.MathUtil.Lerp(prevValue, nextValue, (elapsedTime - prevTime) / (nextTime - prevTime));
						}

						outputProperties[track.Id].Value = currentValue;
					}
					else if (track is FrostySdk.Ebx.TransformPropertyTrackData)
					{
						TransformPropertyTrackData transformTrack = track as FrostySdk.Ebx.TransformPropertyTrackData;
						LinearTransform currentValue = new LinearTransform();

						if (index + 1 >= track.Times.Count)
						{
							currentValue = transformTrack.Values[index];

							if (Data.Looping)
							{
								timer.Restart();
							}
							else
							{
								sequenceStarted = false;
								timer.Reset();

								CallEvent(Event_OnFinished);
							}
						}
						else if (index == -1)
						{
							currentValue = transformTrack.Values[0];
						}
						else
						{
							LinearTransform prevValue = transformTrack.Values[index];
							LinearTransform nextValue = transformTrack.Values[index + 1];
							float prevTime = track.Times[index];
							float nextTime = track.Times[index + 1];

							SharpDX.Matrix m1 = SharpDXUtils.FromLinearTransform(prevValue);
							SharpDX.Matrix m2 = SharpDXUtils.FromLinearTransform(nextValue);

							Debug.WriteLine($"{prevValue} -> {nextValue}");
							Debug.WriteLine($"{(elapsedTime - prevTime) / (nextTime - prevTime)}: {SharpDX.Matrix.Lerp(m1, m2, (elapsedTime - prevTime) / (nextTime - prevTime))}");

							SharpDX.Matrix result = SharpDX.Matrix.Lerp(m1, m2, (elapsedTime - prevTime) / (nextTime - prevTime));

							currentValue = MakeLinearTransform(result);
						}

						outputProperties[track.Id].Value = currentValue;
					}
				}

				foreach (SequenceEventData sequenceEvent in Data.Events)
				{
					if (prevTime < sequenceEvent.Time && elapsedTime >= sequenceEvent.Time)
					{
						outputEvents[sequenceEvent.Event.Id].Execute();
					}
				}

				prevTime = elapsedTime;
			}
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
			Data.PlaybackSpeed = 1;
        }

        public override void OnDataModified()
        {
            base.OnDataModified();

			outputProperties.Clear();
			foreach (PointerRef trackRef in Data.PropertyTracks)
			{
				PropertyTrackData track = trackRef.GetObjectAs<FrostySdk.Ebx.PropertyTrackData>();
				if (track is FrostySdk.Ebx.FloatPropertyTrackData)
				{
					Property<float> propTrack = new Property<float>(this, track.Id);
					outputProperties.Add(track.Id, propTrack);
				}
				else if (track is FrostySdk.Ebx.TransformPropertyTrackData)
				{
					Property<LinearTransform> propTrack = new Property<LinearTransform>(this, track.Id);
					outputProperties.Add(track.Id, propTrack);
				}
			}

			outputEvents.Clear();
			foreach (SequenceEventData sequenceEvent in Data.Events)
			{
				if (!outputEvents.ContainsKey(sequenceEvent.Event.Id))
				{
					outputEvents.Add(sequenceEvent.Event.Id, new Event<OutputEvent>(this, sequenceEvent.Event.Id));
				}
			}
		}
    }
}

