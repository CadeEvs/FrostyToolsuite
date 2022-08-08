using FrostySdk.Attributes;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlatformSplitterEntityData))]
	public class PlatformSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlatformSplitterEntityData>
	{
        private class PlatformSplitterMockData
        {
            [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Enum, "GamePlatform")]
            public FrostySdk.Ebx.GamePlatform MockPlatform { get; set; } = FrostySdk.Ebx.GamePlatform.GamePlatform_Win32;
        }

        protected readonly int Event_Impulse = Frosty.Hash.Fnv1.HashString("Impulse");
        protected readonly int Event_Platform_Win32 = Frosty.Hash.Fnv1.HashString("Platform_Win32");
        protected readonly int Event_Platform_Gen4a = Frosty.Hash.Fnv1.HashString("Platform_Gen4a");
        protected readonly int Event_Platform_Gen4b = Frosty.Hash.Fnv1.HashString("Platform_Gen4b");
        protected readonly int Event_Platform_Android = Frosty.Hash.Fnv1.HashString("Platform_Android");
        protected readonly int Event_Platform_iOS = Frosty.Hash.Fnv1.HashString("Platform_iOS");
        protected readonly int Event_Platform_OSX = Frosty.Hash.Fnv1.HashString("Platform_OSX");
        protected readonly int Event_Platform_Linux = Frosty.Hash.Fnv1.HashString("Platform_Linux");
        protected readonly int Event_Platform_Any = Frosty.Hash.Fnv1.HashString("Platform_Any");

        public new FrostySdk.Ebx.PlatformSplitterEntityData Data => data as FrostySdk.Ebx.PlatformSplitterEntityData;
		public override string DisplayName => "PlatformSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Impulse", Direction.In),
                new ConnectionDesc("Platform_Win32", Direction.Out),
                new ConnectionDesc("Platform_Gen4a", Direction.Out),
                new ConnectionDesc("Platform_Gen4b", Direction.Out),
                new ConnectionDesc("Platform_Android", Direction.Out),
                new ConnectionDesc("Platform_iOS", Direction.Out),
                new ConnectionDesc("Platform_OSX", Direction.Out),
                new ConnectionDesc("Platform_Linux", Direction.Out),
                new ConnectionDesc("Platform_Any", Direction.Out),
                new ConnectionDesc("0xcd7e88ee", Direction.Out)
            };
        }

        protected Event<InputEvent> impulseEvent;
        protected Event<OutputEvent> platformWin32Event;
        protected Event<OutputEvent> platformGen4aEvent;
        protected Event<OutputEvent> platformGen4bEvent;
        protected Event<OutputEvent> platformAndroidEvent;
        protected Event<OutputEvent> platformiOSEvent;
        protected Event<OutputEvent> platformOSXEvent;
        protected Event<OutputEvent> platformLinuxEvent;
        protected Event<OutputEvent> platformAnyEvent;

        private static PlatformSplitterMockData staticMockData = new PlatformSplitterMockData();

        public PlatformSplitterEntity(FrostySdk.Ebx.PlatformSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            mockDataObject = staticMockData;

            impulseEvent = new Event<InputEvent>(this, Event_Impulse);
            platformWin32Event = new Event<OutputEvent>(this, Event_Platform_Win32);
            platformGen4aEvent = new Event<OutputEvent>(this, Event_Platform_Gen4a);
            platformGen4bEvent = new Event<OutputEvent>(this, Event_Platform_Gen4b);
            platformAndroidEvent = new Event<OutputEvent>(this, Event_Platform_Android);
            platformiOSEvent = new Event<OutputEvent>(this, Event_Platform_iOS);
            platformOSXEvent = new Event<OutputEvent>(this, Event_Platform_OSX);
            platformLinuxEvent = new Event<OutputEvent>(this, Event_Platform_Linux);
            platformAnyEvent = new Event<OutputEvent>(this, Event_Platform_Any);
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == impulseEvent.NameHash)
            {
                switch ((mockDataObject as PlatformSplitterMockData).MockPlatform)
                {
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Win32: platformWin32Event.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Gen4a: platformGen4aEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Gen4b: platformGen4bEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Android: platformAndroidEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_iOS: platformiOSEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_OSX: platformOSXEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Linux: platformLinuxEvent.Execute(); return;
                    case FrostySdk.Ebx.GamePlatform.GamePlatform_Any: platformAnyEvent.Execute(); return;
                }
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

