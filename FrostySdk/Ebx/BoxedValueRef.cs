using FrostySdk.Attributes;
using FrostySdk.IO;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace FrostySdk.Ebx
{
    public class BoxedValueWrapperClass
    {
        public object Value { get; set; }
    }

    public class BoxedValueRefConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BoxedValueRef boxedValue = value as BoxedValueRef;
            return new BoxedValueWrapperClass { Value = boxedValue.Value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BoxedValueRef boxedValue = parameter as BoxedValueRef;
            BoxedValueWrapperClass wrapper = value as BoxedValueWrapperClass;
            boxedValue.SetValue(wrapper.Value);
            return boxedValue;
            //throw new NotImplementedException();
        }
    }

    [ClassConverter(typeof(BoxedValueRefConverter))]
    public class BoxedValueRef
    {
        public object Value => value;
        public EbxFieldType Type => type;
        public EbxFieldType ArrayType => subType;
        public EbxFieldCategory Category => category;
        public string TypeString
        {
            get
            {
                switch (type)
                {
                    case EbxFieldType.Array: return EbxTypeToString(subType, value.GetType().GenericTypeArguments[0]);
                    case EbxFieldType.Enum:
                    case EbxFieldType.Struct:
                        return value.GetType().Name;
                    case EbxFieldType.CString: return "CString";
                    default: return type.ToString();
                }
            }
        }

        private object value;
        private EbxFieldType type;
        private EbxFieldType subType;
        private EbxFieldCategory category;

        public BoxedValueRef()
        {
        }

        public BoxedValueRef(object inval, EbxFieldType intype)
        {
            value = inval;
            type = intype;
        }

        public BoxedValueRef(object inval, EbxFieldType intype, EbxFieldType insubtype)
        {
            value = inval;
            type = intype;
            subType = insubtype;
        }

        public BoxedValueRef(object inval, EbxFieldType intype, EbxFieldType insubtype, EbxFieldCategory incategory)
        {
            value = inval;
            type = intype;
            subType = insubtype;
            category = incategory;
        }

        public void SetValue(object invalue)
        {
            value = invalue;
        }

        public override string ToString()
        {
            if (Value == null)
                return "BoxedValueRef '(null)'";
            string s = "BoxedValueRef '";
            switch (type)
            {
                case EbxFieldType.Array: s += $"Array<{EbxTypeToString(subType, value.GetType().GenericTypeArguments[0])}>"; break;
                case EbxFieldType.Enum: s += $"{value.GetType().Name}"; break;
                case EbxFieldType.Struct: s += $"{value.GetType().Name}"; break;
                case EbxFieldType.CString: s += "CString"; break;
                default: s += $"{type}"; break;
            }

            return s + "'";
        }

        private string EbxTypeToString(EbxFieldType typeToConvert, Type actualType)
        {
            switch (typeToConvert)
            {
                case EbxFieldType.Enum:
                case EbxFieldType.Struct:
                    return actualType.Name;
                case EbxFieldType.CString: return "CString";
                default: return typeToConvert.ToString();
            }
        }

        private string StructToString(object structValue)
        {
            StringBuilder sb = new StringBuilder();
            if (TypeLibrary.IsSubClassOf(structValue, "LinearTransform"))
            {
                IValueConverter valueConverter = (IValueConverter)Activator.CreateInstance(System.Type.GetType("Frosty.Core.Controls.Editors.LinearTransformConverter, FrostyCore"));
                structValue = valueConverter.Convert(structValue, null, null, null);
            }

            var pis = structValue.GetType().GetProperties();
            foreach (var pi in pis)
            {
                if (pi.GetCustomAttribute<IsHiddenAttribute>() != null)
                    continue;

                sb.Append(pi.GetValue(structValue).ToString() + "/");
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
