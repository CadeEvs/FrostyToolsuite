using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectObjectEntityData))]
	public class SelectObjectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SelectObjectEntityData>
	{
        protected readonly int Property_InputSelect = Frosty.Hash.Fnv1.HashString("InputSelect");
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectObjectEntityData Data => data as FrostySdk.Ebx.SelectObjectEntityData;
		public override string DisplayName => "SelectObject";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                for (int i = 0; i < Data.Inputs.Count; i++)
                {
                    outEvents.Add(new ConnectionDesc() { Name = $"Select{i} {Data.Inputs[i]}", Direction = Direction.In });
                }
                return outEvents;
            }
        }

        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                for (int i = 0; i < Data.Inputs.Count; i++)
                {
                    outProperties.Add(new ConnectionDesc($"In{i} {Data.Inputs[i]}", Direction.In, (dynamicInputDataType != null) ? dynamicInputDataType : null));
                }
                outProperties.Add(new ConnectionDesc("Out", Direction.Out, (dynamicInputDataType != null) ? dynamicInputDataType : null));
                return outProperties;
            }
        }
        public override IEnumerable<string> HeaderRows
        {
            get
            {
                List<string> outHeaderRows = new List<string>();
                if (dynamicInputDataType != null)
                {
                    outHeaderRows.Add(dynamicInputDataType.Name);
                }
                return outHeaderRows;
            }
        }

        protected Type dynamicInputDataType;
        protected Property<int> inputSelectProperty;
        protected List<IProperty> inProperties = new List<IProperty>();
        protected IProperty outProperty;
        protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

        public SelectObjectEntity(FrostySdk.Ebx.SelectObjectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            dynamicInputDataType = FrostySdk.TypeLibrary.GetType(Data.DynamicInputDataType);

            for (int i = 0; i < Data.Inputs.Count; i++)
            {
                int selectHash = Frosty.Hash.Fnv1.HashString($"Select{i} {Data.Inputs[i]}");
                int hash = Frosty.Hash.Fnv1.HashString($"In{i} {Data.Inputs[i]}");

                inProperties.Add(new Property<object>(this, hash));
                selectEvents.Add(new Event<InputEvent>(this, selectHash));

            }

            inputSelectProperty = new Property<int>(this, Property_InputSelect);
            outProperty = new Property<object>(this, Property_Out);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (inputSelectProperty.IsUnset)
            {
                inputSelectProperty.Value = Data.InputSelect;
            }
        }

        public override void OnEvent(int eventHash)
        {
            int index = selectEvents.FindIndex(e => e.NameHash == eventHash);
            if (index != -1)
            {
                inputSelectProperty.Value = index;
                return;
            }

            base.OnEvent(eventHash);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            int index = inProperties.FindIndex(p => p.NameHash == propertyHash);
            if (index != -1)
            {
                outProperty.Value = inProperties[inputSelectProperty.Value].Value;
                return;
            }
            else if (propertyHash == inputSelectProperty.NameHash)
            {
                outProperty.Value = inProperties[inputSelectProperty.Value].Value;
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnDataModified()
        {
            base.OnDataModified();

            var type = FrostySdk.TypeLibrary.GetType(Data.DynamicInputDataType);
            if (type != dynamicInputDataType)
            {
                dynamicInputDataType = type;
            }
        }
    }
}

