using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LevelEditorPlugin.Entities
{
    public class ControllableTimer
    {
        public double ElapsedTime
        {
            get
            {
                return (elapsedTime + timer.Elapsed).TotalSeconds * playbackRate;
            }
        }
        public double ExternalTime
        {
            set
            {
                if (infinite)
                {
                    elapsedTime = TimeSpan.FromSeconds(value * playbackRate);
                }
            }
        }
        public bool IsRunning
        {
            get => isRunning;
        }
        private Stopwatch timer;
        private double startTime;
        private double endTime;
        private double jumpTime;
        private double playbackRate;
        private bool playOnce;
        private bool playOnceDone;
        private bool infinite;
        private TimeSpan elapsedTime;
        private bool isRunning;

        public ControllableTimer(double inStartTime, double inEndTime, double inJumpTime, bool inPlayOnce = false, bool inInfinite = false, double inPlaybackRate = 1.0)
        {
            timer = new Stopwatch();
            startTime = inStartTime;
            endTime = inEndTime;
            jumpTime = inJumpTime;
            playOnce = inPlayOnce;
            infinite = inInfinite;
            playbackRate = inPlaybackRate;
        }

        public void Start()
        {
            if (playOnce && playOnceDone)
                return;

            isRunning = true;
            if (infinite)
                return;

            timer.Start();
            playOnceDone = true;
        }

        public void Stop()
        {
            isRunning = false;
            if (infinite)
                return;

            timer.Stop();
        }

        public void Restart()
        {
            if (infinite)
                return;

            timer.Restart();
        }

        public void Reset()
        {
            isRunning = false;
            if (infinite)
                return;

            playOnceDone = false;
            timer.Reset();
        }

        public void GoToStart()
        {
            elapsedTime = TimeSpan.FromSeconds(startTime);
        }

        public void GoToEnd()
        {
            elapsedTime = TimeSpan.FromSeconds(endTime);
        }

        public void GoToJump()
        {
            elapsedTime = TimeSpan.FromSeconds(jumpTime);
        }
    }

	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimelineEntityData))]
	public class TimelineEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TimelineEntityData>, IContainerOfEntities
	{
        protected readonly int Event_Play = Frosty.Hash.Fnv1.HashString("Play");
        protected readonly int Event_Pause = Frosty.Hash.Fnv1.HashString("Pause");
        protected readonly int Event_Resume = Frosty.Hash.Fnv1.HashString("Resume");
        protected readonly int Event_Stop = Frosty.Hash.Fnv1.HashString("Stop");
        protected readonly int Event_GoToJump = Frosty.Hash.Fnv1.HashString("GoToJump");
        protected readonly int Event_GoToStart = Frosty.Hash.Fnv1.HashString("GoToStart");
        protected readonly int Event_GoToEnd = Frosty.Hash.Fnv1.HashString("GoToEnd");

        protected readonly int Property_PlaybackRate = Frosty.Hash.Fnv1.HashString("PlaybackRate");
        protected readonly int Property_ExternalTime = Frosty.Hash.Fnv1.HashString("ExternalTime");

        public new FrostySdk.Ebx.TimelineEntityData Data => data as FrostySdk.Ebx.TimelineEntityData;
		public override string DisplayName => "Timeline";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.AddRange(new[]
                {
                    new ConnectionDesc("Play", Direction.In),
                    new ConnectionDesc("Pause", Direction.In),
                    new ConnectionDesc("Resume", Direction.In),
                    new ConnectionDesc("Stop", Direction.In),
                    new ConnectionDesc("GoToStart", Direction.In),
                    new ConnectionDesc("GoToEnd", Direction.In),
                    new ConnectionDesc("GoToJump", Direction.In),

                    new ConnectionDesc("BufferingStart", Direction.In),
                    new ConnectionDesc("BufferingCancel", Direction.In),

                    new ConnectionDesc("OnStarted", Direction.Out),
                    new ConnectionDesc("OnStopped", Direction.Out),
                    new ConnectionDesc("OnFinished", Direction.Out),

                    new ConnectionDesc("OnPaused", Direction.Out),
                    new ConnectionDesc("OnUnpaused", Direction.Out),
                    new ConnectionDesc("OnShotChanged", Direction.Out)

                });

                List<ConnectionDesc> tmpEvents = new List<ConnectionDesc>();
                foreach (var track in tracks)
                {
                    tmpEvents.AddRange(track.Events);
                }
                outEvents.AddRange(tmpEvents.Distinct());
                return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(new[]
                {
                    new ConnectionDesc("InitTime", Direction.In),
                    new ConnectionDesc("ExternalTime", Direction.In),
                    new ConnectionDesc("PlaybackRate", Direction.In),
                    new ConnectionDesc("ObjectLayersMask", Direction.In),
                    new ConnectionDesc("TimelineOrigin", Direction.In),
                    new ConnectionDesc("LocalPlayerId", Direction.In),

                    new ConnectionDesc("CurrentTime", Direction.Out),

                    new ConnectionDesc("CurrentShot", Direction.Out),
                    new ConnectionDesc("CurrentTransform", Direction.Out)
                });

                List<ConnectionDesc> tmpProperties = new List<ConnectionDesc>();
                foreach (var track in tracks)
                {
                    tmpProperties.AddRange(track.Properties);
                }
                outProperties.AddRange(tmpProperties.Distinct());
                return outProperties;
            }
        }
        public override IEnumerable<string> DebugRows
        {
            get
            {
                List<string> outDebugRows = new List<string>();
                if (timer.IsRunning)
                {
                    outDebugRows.Add($"PlayTime: {TimeSpan.FromSeconds(timer.ElapsedTime)}");
                }
                return outDebugRows;
            }
        }
        public string TrackName => "Timeline";
        public IEnumerable<TimelineTrack> Tracks => tracks;

        protected Event<InputEvent> playEvent;
        protected Event<InputEvent> pauseEvent;
        protected Event<InputEvent> resumeEvent;
        protected Event<InputEvent> stopEvent;
        protected Event<InputEvent> gotoJumpEvent;
        protected Event<InputEvent> gotoStartEvent;
        protected Event<InputEvent> gotoEndEvent;

        protected Property<float> playbackRateProperty;
#if MASS_EFFECT
        protected Property<float> externalTimeProperty;
#endif

        private List<TimelineTrack> tracks = new List<TimelineTrack>();
        private ControllableTimer timer;

        public TimelineEntity(FrostySdk.Ebx.TimelineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            var timelineData = Data.TimelineData.GetObjectAs<FrostySdk.Ebx.TimelineData>();
            foreach (var objPointer in timelineData.Children)
            {
                var objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                var track = CreateEntity(objectData);

                if (track != null)
                {
                    var timelineTrack = track as TimelineTrack;
                    timelineTrack.InitializeForSchematics(this);

                    tracks.Add(timelineTrack);
                }
            }

            playbackRateProperty = new Property<float>(this, Property_PlaybackRate, Data.PlaybackRate);
#if MASS_EFFECT
            externalTimeProperty = new Property<float>(this, Property_ExternalTime, Data.ExternalTime);
#endif

            playEvent = new Event<InputEvent>(this, Event_Play);
            pauseEvent = new Event<InputEvent>(this, Event_Pause);
            resumeEvent = new Event<InputEvent>(this, Event_Resume);
            stopEvent = new Event<InputEvent>(this, Event_Stop);
            gotoJumpEvent = new Event<InputEvent>(this, Event_GoToJump);
            gotoStartEvent = new Event<InputEvent>(this, Event_GoToStart);
            gotoEndEvent = new Event<InputEvent>(this, Event_GoToEnd);

#if MASS_EFFECT
            timer = new ControllableTimer(Data.StartTime, Data.EndTime, Data.JumpTime, false, Data.Infinite, Data.PlaybackRate);
#else
            timer = new ControllableTimer(Data.StartTime, Data.EndTime, 0, false, Data.Infinite, Data.PlaybackRate);
#endif
        }

        public void AddEntity(Entity inEntity)
        {
        }

        public void RemoveEntity(Entity inEntity)
        {
        }

        public override void Destroy()
        {
            foreach (var track in tracks)
            {
                track.Destroy();
            }    

            base.Destroy();
        }

        public Entity FindEntity(Guid instanceGuid)
        {
            foreach (var track in tracks)
            {
                if (track.InstanceGuid == instanceGuid)
                    return track;

                var entity = track.FindEntity(instanceGuid);
                if (entity != null)
                    return entity;
            }

            return null;
        }

        public void Update(float elapsedTime)
        {
            foreach (var track in tracks)
            {
                track.Update(elapsedTime);
            }
        }

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (Data.AutoPlay)
            {
                timer.Start();
            }
        }
        public override void Update_PostFrame()
        {
            if (timer.IsRunning)
            {
                double elapsedTime = timer.ElapsedTime;
                Update((float)elapsedTime);

                if (elapsedTime >= Data.EndTime)
                {
                    if (!Data.Infinite)
                    {
                        if (Data.Looping)
                        {
                            timer.Restart();
                        }
                        else
                        {
                            timer.Reset();
                        }
                    }
                }
            }
        }

        public override void OnPropertyChanged(int propertyHash)
        {
#if MASS_EFFECT
            if (propertyHash == externalTimeProperty.NameHash)
            {
                timer.ExternalTime = externalTimeProperty.Value;
                return;
            }
#endif

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == playEvent.NameHash)
            {
                timer.Start();
                return;
            }
            else if (eventHash == pauseEvent.NameHash)
            {
                timer.Stop();
                return;
            }
            else if (eventHash == resumeEvent.NameHash)
            {
                timer.Start();
                return;
            }
            else if (eventHash == stopEvent.NameHash)
            {
                timer.Stop();
                timer.Reset();
                return;
            }
            else if (eventHash == gotoJumpEvent.NameHash)
            {
                timer.GoToJump();
                return;
            }
            else if (eventHash == gotoStartEvent.NameHash)
            {
                timer.GoToStart();
                return;
            }
            else if (eventHash == gotoEndEvent.NameHash)
            {
                timer.GoToEnd();
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

