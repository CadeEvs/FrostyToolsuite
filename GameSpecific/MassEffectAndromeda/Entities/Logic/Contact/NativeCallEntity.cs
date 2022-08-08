using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	public enum NativeCallTypes
    {
		GetCurrentProfileName = 1178356822
	}

	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NativeCallEntityData))]
	public class NativeCallEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NativeCallEntityData>
	{
		protected readonly int Event_Execute = Frosty.Hash.Fnv1.HashString("Execute");

		public new FrostySdk.Ebx.NativeCallEntityData Data => data as FrostySdk.Ebx.NativeCallEntityData;
		public override string DisplayName => "NativeCall";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				foreach (var fieldHash in Data.OutputFields)
				{
					outProperties.Add(new ConnectionDesc(FrostySdk.Utils.GetString((int)fieldHash), Direction.Out));
				}
				return outProperties;
            }
        }
        public override IEnumerable<string> HeaderRows
        {
			get => new List<string>
			{
				FrostySdk.Utils.GetString((int)Data.NativeCallType)
			};
        }

        protected List<IProperty> outputProperties = new List<IProperty>();
		protected Event<InputEvent> executeEvent;

        public NativeCallEntity(FrostySdk.Ebx.NativeCallEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			foreach (var fieldHash in Data.OutputFields)
			{
				outputProperties.Add(new Property<object>(this, (int)fieldHash));
			}
			executeEvent = new Event<InputEvent>(this, Event_Execute);
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == executeEvent.NameHash)
			{
				ExecuteNativeFunction();
				return;
			}

            base.OnEvent(eventHash);
        }

		private void ExecuteNativeFunction()
		{
			switch ((NativeCallTypes)Data.NativeCallType)
			{
				case NativeCallTypes.GetCurrentProfileName:
                {
					var property = GetProperty(Frosty.Hash.Fnv1.HashString("CurrentProfileName"));
					property.Value = (CString)"Frosty";
                }
				break;
			}
		}
    }
}

