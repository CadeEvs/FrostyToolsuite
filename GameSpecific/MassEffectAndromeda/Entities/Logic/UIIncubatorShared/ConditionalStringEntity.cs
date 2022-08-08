using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalStringEntityData))]
	public class ConditionalStringEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalStringEntityData>
	{
		protected readonly int Property_ValueIfTrue = Frosty.Hash.Fnv1.HashString("ValueIfTrue");
		protected readonly int Property_ValueIfFalse = Frosty.Hash.Fnv1.HashString("ValueIfFalse");
		protected readonly int Property_Output = Frosty.Hash.Fnv1.HashString("Output");

		public new FrostySdk.Ebx.ConditionalStringEntityData Data => data as FrostySdk.Ebx.ConditionalStringEntityData;
		public override string DisplayName => "ConditionalString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("ValueIfTrue", Direction.In, typeof(CString)));
				outProperties.Add(new ConnectionDesc("ValueIfFalse", Direction.In, typeof(CString)));
				outProperties.Add(new ConnectionDesc("Output", Direction.Out, typeof(CString)));
				return outProperties;
			}
		}

		protected Property<CString> valueIfTrueProperty;
		protected Property<CString> valueIfFalseProperty;
		protected Property<CString> outputProperty;

		public ConditionalStringEntity(FrostySdk.Ebx.ConditionalStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			valueIfTrueProperty = new Property<CString>(this, Property_ValueIfTrue, Data.ValueIfTrue);
			valueIfFalseProperty = new Property<CString>(this, Property_ValueIfFalse, Data.ValueIfFalse);
			outputProperty = new Property<CString>(this, Property_Output);
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

