using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalIntEntityData))]
	public class ConditionalIntEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalIntEntityData>
	{
		protected readonly int Property_Output = Frosty.Hash.Fnv1.HashString("Output");
		protected readonly int Property_ValueIfTrue = Frosty.Hash.Fnv1.HashString("ValueIfTrue");
		protected readonly int Property_ValueIfFalse = Frosty.Hash.Fnv1.HashString("ValueIfFalse");

		public new FrostySdk.Ebx.ConditionalIntEntityData Data => data as FrostySdk.Ebx.ConditionalIntEntityData;
		public override string DisplayName => "ConditionalInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("ValueIfTrue", Direction.In, typeof(int)));
				outProperties.Add(new ConnectionDesc("ValueIfFalse", Direction.In, typeof(int)));
				outProperties.Add(new ConnectionDesc("Output", Direction.Out, typeof(int)));
				return outProperties;
			}
		}

		protected Property<int> valueIfTrueProperty;
		protected Property<int> valueIfFalseProperty;
		protected Property<int> outputProperty;

		public ConditionalIntEntity(FrostySdk.Ebx.ConditionalIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			valueIfTrueProperty = new Property<int>(this, Property_ValueIfTrue, Data.ValueIfTrue);
			valueIfFalseProperty = new Property<int>(this, Property_ValueIfFalse, Data.ValueIfFalse);
			outputProperty = new Property<int>(this, Property_Output);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == conditionProperty.NameHash)
			{
				bool condition = conditionProperty.Value;
				outputProperty.Value = (condition)
					? valueIfTrueProperty.Value
					: valueIfFalseProperty.Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

