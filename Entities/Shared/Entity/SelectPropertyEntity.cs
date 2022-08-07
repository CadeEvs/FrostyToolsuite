using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectPropertyEntityData))]
	public class SelectPropertyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SelectPropertyEntityData>
	{
        protected readonly int Property_InputSelect = Frosty.Hash.Fnv1.HashString("InputSelect");
        protected readonly int Property_SelectedIndex = Frosty.Hash.Fnv1.HashString("SelectedIndex");

        public new FrostySdk.Ebx.SelectPropertyEntityData Data => data as FrostySdk.Ebx.SelectPropertyEntityData;
		public override string DisplayName => "SelectProperty";
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
                outEvents.Add(new ConnectionDesc("OnSelected", Direction.Out));
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
                    outProperties.Add(new ConnectionDesc() { Name = $"In{i} {Data.Inputs[i]}", Direction = Direction.In });
                }
                outProperties.Add(new ConnectionDesc("InputSelect", Direction.In, typeof(int)));
                outProperties.Add(new ConnectionDesc("SelectedIndex", Direction.Out, typeof(int)));
                return outProperties;
            }
        }
        public override IEnumerable<string> DebugRows
        {
            get
            {
                List<string> outDebugRows = new List<string>();
                outDebugRows.Add($"InputSelect: {inputSelectProperty.Value}");
                return outDebugRows;
            }
        }

        protected Property<int> inputSelectProperty;
        protected Property<int> selectedIndexProperty;

        public SelectPropertyEntity(FrostySdk.Ebx.SelectPropertyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            inputSelectProperty = new Property<int>(this, Property_InputSelect, Data.InputSelect);
            selectedIndexProperty = new Property<int>(this, Property_SelectedIndex);
        }

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (inputSelectProperty.IsUnset)
            {
                inputSelectProperty.Value = Data.InputSelect;
            }
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            if (propertyHash == inputSelectProperty.NameHash)
            {
                selectedIndexProperty.Value = inputSelectProperty.Value;
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }
    }
}

