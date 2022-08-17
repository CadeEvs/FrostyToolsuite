using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrEntityData))]
	public class OrEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OrEntityData>
	{
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.OrEntityData Data => data as FrostySdk.Ebx.OrEntityData;
		public override string DisplayName => "Or";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                for (int i = 0; i < Data.InputCount; i++)
                {
                    outProperties.Add(new ConnectionDesc($"In{i + 1}", Direction.In, typeof(bool)));
                }
                outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(bool)));
                return outProperties;
            }
        }

        protected List<Property<bool>> inProperties = new List<Property<bool>>();
        protected Property<bool> outProperty;

        public OrEntity(FrostySdk.Ebx.OrEntityData inData, Entity inParent)
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
                    retVal |= inProperties[i].Value;
                }
                outProperty.Value = retVal;
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }
    }
}

