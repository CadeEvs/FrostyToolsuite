using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimerEntityData))]
	public class TimerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TimerEntityData>
	{
        protected readonly int Event_Play = Frosty.Hash.Fnv1.HashString("Play");
        protected readonly int Event_Pause = Frosty.Hash.Fnv1.HashString("Pause");
        protected readonly int Event_Reset = Frosty.Hash.Fnv1.HashString("Reset");
        protected readonly int Event_OnMax = Frosty.Hash.Fnv1.HashString("OnMax");
        protected readonly int Property_Max = Frosty.Hash.Fnv1.HashString("Max");

		public new FrostySdk.Ebx.TimerEntityData Data => data as FrostySdk.Ebx.TimerEntityData;
		public override string DisplayName => "Timer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Play", Direction.In),
                new ConnectionDesc("Pause", Direction.In),
                new ConnectionDesc("Reset", Direction.In),
                new ConnectionDesc("Jump", Direction.In),
                new ConnectionDesc("OnMax", Direction.Out)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Min", Direction.In),
                new ConnectionDesc("Max", Direction.In),
                new ConnectionDesc("StartTime", Direction.In),
                new ConnectionDesc("JumpTime", Direction.In),
                new ConnectionDesc("Speed", Direction.In),
                new ConnectionDesc("Time", Direction.Out)
            };
        }
        public override IEnumerable<string> DebugRows
        {
            get
            {
                List<string> outDebugRows = new List<string>();
                if (timer.IsRunning)
                {
                    outDebugRows.Add($"Time: {TimeSpan.FromSeconds(timer.ElapsedTime)}");
                }
                return outDebugRows;
            }
        }

        protected Event<InputEvent> playEvent;
        protected Event<InputEvent> pauseEvent;
        protected Event<InputEvent> resetEvent;
        protected Event<OutputEvent> onMaxEvent;
        protected Property<float> maxProperty;

        private ControllableTimer timer;

        public TimerEntity(FrostySdk.Ebx.TimerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            playEvent = new Event<InputEvent>(this, Event_Play);
            pauseEvent = new Event<InputEvent>(this, Event_Pause);
            resetEvent = new Event<InputEvent>(this, Event_Reset);
            onMaxEvent = new Event<OutputEvent>(this, Event_OnMax);
            maxProperty = new Property<float>(this, Property_Max, Data.Max);

            timer = new ControllableTimer(Data.StartTime, Data.Max, Data.JumpTime, false, false, Data.Speed);
		}

        public override void Update_PreFrame()
        {
            base.Update_PreFrame();
            if (Data.UpdatePass == FrostySdk.Ebx.UpdatePass.UpdatePass_PreSim)
            {
                UpdateTimer();
            }
        }

        public override void Update_PostFrame()
        {
            base.Update_PostFrame();
            if (Data.UpdatePass != FrostySdk.Ebx.UpdatePass.UpdatePass_PreSim)
            {
                UpdateTimer();
            }
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
            else if (eventHash == resetEvent.NameHash)
            {
                timer.Reset();
                return;
            }
            base.OnEvent(eventHash);
        }

        private void UpdateTimer()
        {
            if (timer.IsRunning)
            {
                if (timer.ElapsedTime >= maxProperty.Value)
                {
                    onMaxEvent.Execute();

                    if (Data.RestartOnGoal)
                    {
                        timer.Reset();
                    }

                    if (Data.Looping)
                    {
                        timer.Start();
                    }
                    else
                    {
                        timer.Stop();
                    }
                }
            }
        }
    }
}

