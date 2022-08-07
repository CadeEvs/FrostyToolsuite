using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AndEntityData))]
	public class AndEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AndEntityData>
	{
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.AndEntityData Data => data as FrostySdk.Ebx.AndEntityData;
		public override string DisplayName => "And";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                for (int i = 0; i < Data.InputCount; i++)
                {
                    outProperties.Add(new ConnectionDesc() { Name = $"In{i + 1}", Direction = Direction.In });
                }
                outProperties.Add(new ConnectionDesc("Out", Direction.Out));
                return outProperties;
            }
        }

        protected List<Property<bool>> inProperties = new List<Property<bool>>();
        protected Property<bool> outProperty;

        public AndEntity(FrostySdk.Ebx.AndEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            for (int i = 0; i < Data.InputCount; i++)
            {
                inProperties.Add(new Property<bool>(this, Frosty.Hash.Fnv1.HashString($"In{i + 1}")));
            }
            outProperty = new Property<bool>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
            int index = inProperties.FindIndex(p => p.NameHash == propertyHash);
            if (index != -1)
            {
                bool retVal = inProperties[0].Value;
                for (int i = 1; i < inProperties.Count; i++)
                {
                    retVal &= inProperties[i].Value;
                }
                outProperty.Value = retVal;
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }
    }
}

