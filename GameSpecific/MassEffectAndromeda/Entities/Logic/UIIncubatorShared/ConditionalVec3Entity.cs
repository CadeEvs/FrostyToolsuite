using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalVec3EntityData))]
	public class ConditionalVec3Entity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalVec3EntityData>
	{
		protected readonly int Property_ValueIfTrue = Frosty.Hash.Fnv1.HashString("ValueIfTrue");
		protected readonly int Property_ValueIfFalse = Frosty.Hash.Fnv1.HashString("ValueIfFalse");
		protected readonly int Property_Output = Frosty.Hash.Fnv1.HashString("Output");

		public new FrostySdk.Ebx.ConditionalVec3EntityData Data => data as FrostySdk.Ebx.ConditionalVec3EntityData;
		public override string DisplayName => "ConditionalVec3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("ValueIfTrue", Direction.In, typeof(Vec3)));
				outProperties.Add(new ConnectionDesc("ValueIfFalse", Direction.In, typeof(Vec3)));
				outProperties.Add(new ConnectionDesc("Output", Direction.Out, typeof(Vec3)));
				return outProperties;
			}
		}

		protected Property<Vec3> valueIfTrueProperty;
		protected Property<Vec3> valueIfFalseProperty;
		protected Property<Vec3> outputProperty;

		public ConditionalVec3Entity(FrostySdk.Ebx.ConditionalVec3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			valueIfTrueProperty = new Property<Vec3>(this, Property_ValueIfTrue, Data.ValueIfTrue);
			valueIfFalseProperty = new Property<Vec3>(this, Property_ValueIfFalse, Data.ValueIfFalse);
			outputProperty = new Property<Vec3>(this, Property_Output);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == conditionProperty.NameHash || propertyHash == valueIfFalseProperty.NameHash || propertyHash == valueIfTrueProperty.NameHash)
			{
				if (conditionProperty.Value) outputProperty.Value = valueIfTrueProperty.Value;
				else outputProperty.Value = valueIfFalseProperty.Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

