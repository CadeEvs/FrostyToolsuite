using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectStringEntityData))]
	public class SelectStringEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectStringEntityData>
	{
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

        public new FrostySdk.Ebx.SelectStringEntityData Data => data as FrostySdk.Ebx.SelectStringEntityData;
		public override string DisplayName => "SelectString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(base.Properties);
                outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(CString)));
                return outProperties;
            }
        }

        protected List<Property<CString>> inputProperties = new List<Property<CString>>();
        protected Property<CString> outProperty;

        protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

        public SelectStringEntity(FrostySdk.Ebx.SelectStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            for (int i = 0; i < inData.Inputs.Count; i++)
            {
                int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
                Property<CString> property = new Property<CString>(this, Frosty.Hash.Fnv1.HashString($"In{i} {inData.Inputs[i]}"));

                inputProperties.Add(property);
                selectEvents.Add(new Event<InputEvent>(this, selectEvent));
            }

            outProperty = new Property<CString>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
            
            if (propertyHash == inputProperties[inputSelectProperty.Value].NameHash)
            {
                outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
                return;
            }
            else if (propertyHash == inputSelectProperty.NameHash)
            {
                outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
            }

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnEvent(int eventHash)
        {
            int eventIndex = selectEvents.FindIndex(e => e.NameHash == eventHash);
            if (eventIndex != -1)
            {
                inputSelectProperty.Value = eventIndex;
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

