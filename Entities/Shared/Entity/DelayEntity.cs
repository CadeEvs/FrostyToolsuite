using System.Collections.Generic;
using System.Diagnostics;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DelayEntityData))]
	public class DelayEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DelayEntityData>
	{
        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_Restart = Frosty.Hash.Fnv1.HashString("Restart");
        protected readonly int Event_Reset = Frosty.Hash.Fnv1.HashString("Reset");
        protected readonly int Event_Out = Frosty.Hash.Fnv1.HashString("Out");

        protected readonly int Property_Delay = Frosty.Hash.Fnv1.HashString("Delay");

        public new FrostySdk.Ebx.DelayEntityData Data => data as FrostySdk.Ebx.DelayEntityData;
		public override string DisplayName => "Delay";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("In", Direction.In),
                new ConnectionDesc("Restart", Direction.In),
                new ConnectionDesc("Reset", Direction.In),
                new ConnectionDesc("Out", Direction.Out),
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Delay", Direction.In)
            };
        }
        public override IEnumerable<string> HeaderRows
        {
            get
            {
                List<string> outHeaderRows = new List<string>();
                if (Data.Delay > 0)
                {
                    outHeaderRows.Add($"Delay: {Data.Delay}");
                }
                return outHeaderRows;
            }
        }
        public override IEnumerable<string> DebugRows
        {
            get
            {
                List<string> outDebugRows = new List<string>();
                if (timer.IsRunning)
                {
                    outDebugRows.Add($"Delay: {Data.Delay - timer.Elapsed.TotalSeconds}");
                }
                return outDebugRows;
            }
        }

        protected Property<float> delayProperty;
        protected Event<InputEvent> inEvent;
        protected Event<InputEvent> resetEvent;
        protected Event<InputEvent> restartEvent;
        protected Event<OutputEvent> outEvent;

        protected Stopwatch timer;
        protected int delayRunCount;

        public DelayEntity(FrostySdk.Ebx.DelayEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            delayProperty = new Property<float>(this, Property_Delay, Data.Delay);
            inEvent = new Event<InputEvent>(this, Event_In);
            resetEvent = new Event<InputEvent>(this, Event_Reset);
            restartEvent = new Event<InputEvent>(this, Event_Restart);
            outEvent = new Event<OutputEvent>(this, Event_Out);

            timer = new Stopwatch();
		}

        public override void OnEvent(int eventHash)
        {
            if (eventHash == Event_In)
            {
                if (Data.RunOnce && delayRunCount > 0)
                    return;

                delayRunCount++;
                timer.Start();
                return;
            }
            else if (eventHash == Event_Reset)
            {
                delayRunCount = 0;
                timer.Reset();
                return;
            }
            else if (eventHash == Event_Restart)
            {
                delayRunCount++;
                timer.Restart();
                return;
            }

            base.OnEvent(eventHash);
        }

        public override void Update_PreFrame()
        {
            base.Update_PreFrame();
            if (delayRunCount == 0 && Data.AutoStart)
            {
                delayRunCount++;
                timer.Start();
            }

            if (timer.IsRunning)
            {
                if (timer.Elapsed.TotalSeconds >= delayProperty.Value)
                {
                    if (Data.RunOnce) timer.Stop();
                    else timer.Reset();

                    CallEvent(Event_Out);
                }
            }
        }
    }
}

