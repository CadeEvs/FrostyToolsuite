using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalFloatEntityData))]
	public class ConditionalFloatEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalFloatEntityData>
	{
		protected readonly int Property_Output = Frosty.Hash.Fnv1.HashString("Output");
		protected readonly int Property_ValueIfTrue = Frosty.Hash.Fnv1.HashString("ValueIfTrue");
		protected readonly int Property_ValueIfFalse = Frosty.Hash.Fnv1.HashString("ValueIfFalse");

		public new FrostySdk.Ebx.ConditionalFloatEntityData Data => data as FrostySdk.Ebx.ConditionalFloatEntityData;
		public override string DisplayName => "ConditionalFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("ValueIfTrue", Direction.In, typeof(float)));
				outProperties.Add(new ConnectionDesc("ValueIfFalse", Direction.In, typeof(float)));
				outProperties.Add(new ConnectionDesc("Output", Direction.Out, typeof(float)));
				return outProperties;
			}
        }

		protected Property<float> valueIfTrueProperty;
		protected Property<float> valueIfFalseProperty;
		protected Property<float> outputProperty;

		public ConditionalFloatEntity(FrostySdk.Ebx.ConditionalFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			valueIfTrueProperty = new Property<float>(this, Property_ValueIfTrue, Data.ValueIfTrue);
			valueIfFalseProperty = new Property<float>(this, Property_ValueIfFalse, Data.ValueIfFalse);
			outputProperty = new Property<float>(this, Property_Output);
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

