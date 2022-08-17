using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyCastEntityData))]
	public class PropertyCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyCastEntityData>
	{
        protected readonly int Property_BoolValue = Frosty.Hash.Fnv1.HashString("BoolValue");
        protected readonly int Property_FloatValue = Frosty.Hash.Fnv1.HashString("FloatValue");
        protected readonly int Property_IntValue = Frosty.Hash.Fnv1.HashString("IntValue");
        protected readonly int Property_UintValue = Frosty.Hash.Fnv1.HashString("UintValue");
        protected readonly int Property_StringValue = Frosty.Hash.Fnv1.HashString("StringValue");
        protected readonly int Property_CastToBool = Frosty.Hash.Fnv1.HashString("CastToBool");
        protected readonly int Property_CastToFloat = Frosty.Hash.Fnv1.HashString("CastToFloat");
        protected readonly int Property_CastToInt = Frosty.Hash.Fnv1.HashString("CastToInt");
        protected readonly int Property_CastToUint = Frosty.Hash.Fnv1.HashString("CastToUint");
        protected readonly int Property_CastToString = Frosty.Hash.Fnv1.HashString("CastToString");

        public new FrostySdk.Ebx.PropertyCastEntityData Data => data as FrostySdk.Ebx.PropertyCastEntityData;
		public override string DisplayName => "PropertyCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("BoolValue", Direction.In, typeof(bool)),
                new ConnectionDesc("FloatValue", Direction.In, typeof(float)),
                new ConnectionDesc("IntValue", Direction.In, typeof(int)),
                new ConnectionDesc("UintValue", Direction.In, typeof(uint)),
                new ConnectionDesc("StringValue", Direction.In, typeof(CString)),
                new ConnectionDesc("CastToBool", Direction.Out, typeof(bool)),
                new ConnectionDesc("CastToFloat", Direction.Out, typeof(float)),
                new ConnectionDesc("CastToInt", Direction.Out, typeof(int)),
                new ConnectionDesc("CastToUint", Direction.Out, typeof(uint)),
                new ConnectionDesc("CastToString", Direction.Out, typeof(CString)),
            };
        }

        protected Property<bool> boolValueProperty;
        protected Property<float> floatValueProperty;
        protected Property<int> intValueProperty;
        protected Property<uint> uintValueProperty;
        protected Property<CString> stringValueProperty;
        protected Property<bool> castToBoolProperty;
        protected Property<float> castToFloatProperty;
        protected Property<int> castToIntProperty;
        protected Property<uint> castToUintProperty;
        protected Property<CString> castToStringProperty;

        public PropertyCastEntity(FrostySdk.Ebx.PropertyCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            boolValueProperty = new Property<bool>(this, Property_BoolValue, Data.BoolValue);
            floatValueProperty = new Property<float>(this, Property_FloatValue, Data.FloatValue);
            intValueProperty = new Property<int>(this, Property_IntValue, Data.IntValue);
            uintValueProperty = new Property<uint>(this, Property_UintValue, Data.UintValue);
            stringValueProperty = new Property<CString>(this, Property_StringValue, Data.StringValue);

            castToBoolProperty = new Property<bool>(this, Property_CastToBool, false);
            castToFloatProperty = new Property<float>(this, Property_CastToFloat, 0.0f);
            castToIntProperty = new Property<int>(this, Property_CastToInt, 0);
            castToUintProperty = new Property<uint>(this, Property_CastToUint, 0);
            castToStringProperty = new Property<CString>(this, Property_CastToString, "");
		}

        public override void OnPropertyChanged(int propertyHash)
        {
            if (propertyHash == boolValueProperty.NameHash)
            {
                bool boolValue = boolValueProperty.Value;
                castToBoolProperty.Value = boolValue;
                castToIntProperty.Value = (boolValue) ? 1 : 0;
                castToUintProperty.Value = (uint)((boolValue) ? 1 : 0);
                castToFloatProperty.Value = (float)((boolValue) ? 1 : 0);
                castToStringProperty.Value = (CString)((boolValue) ? "True" : "False");
                return;
            }
            else if (propertyHash == floatValueProperty.NameHash)
            {
                float floatValue = floatValueProperty.Value;
                castToBoolProperty.Value = (floatValue > 0.0f) ? true : false;
                castToIntProperty.Value = (int)floatValue;
                castToUintProperty.Value = (uint)floatValue;
                castToFloatProperty.Value = floatValue;
                castToStringProperty.Value = floatValue.ToString($"F{Data.Precision}");
                return;
            }
            else if (propertyHash == intValueProperty.NameHash)
            {
                int intValue = intValueProperty.Value;
                castToBoolProperty.Value = (intValue > 0) ? true : false;
                castToIntProperty.Value = intValue;
                castToUintProperty.Value = (uint)intValue;
                castToFloatProperty.Value = (float)intValue;
                castToStringProperty.Value = intValue.ToString();
                return;
            }
            else if (propertyHash == uintValueProperty.NameHash)
            {
                uint uintValue = uintValueProperty.Value;
                castToBoolProperty.Value = (uintValue > 0.0f) ? true : false;
                castToIntProperty.Value = (int)uintValue;
                castToUintProperty.Value = uintValue;
                castToFloatProperty.Value = (float)uintValue;
                castToStringProperty.Value = uintValue.ToString();
                return;
            }
            else if (propertyHash == stringValueProperty.NameHash)
            {
                // @todo
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }
    }
}

