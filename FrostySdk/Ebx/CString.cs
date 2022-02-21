using System;

namespace FrostySdk.Ebx
{
    public struct CString
    {
        private string strValue;

        public CString(string value = "") => strValue = value;

        public CString Sanitize() => new CString(strValue.Trim('\v', '\r', '\n', '\t'));

        public static implicit operator string(CString value) => value.strValue ?? (value.strValue = "");

        public static implicit operator CString(string value) => new CString(value);

        public bool IsNull() => strValue == null;

        public override string ToString() => strValue;

        public override int GetHashCode()
        {
            if (strValue == null)
                return "".GetHashCode();

            return strValue.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if (obj is CString b)
            {
                if (strValue == null)
                {
                    return string.IsNullOrEmpty(b.strValue);
                }

                if (b.strValue == null)
                {
                    return string.IsNullOrEmpty(strValue);
                }

                return strValue.Equals(b.strValue);
            }
            
            if (obj is string b1)
            {
                if (strValue == null)
                {
                    return b1 == "";
                }

                return strValue.Equals(b1);
            }
            return false;
        }

        public bool Equals(object obj, StringComparison comparison)
        {
            if (obj is CString b)
            {
                if (strValue == null)
                {
                    return string.IsNullOrEmpty(b.strValue);
                }

                if (b.strValue == null)
                {
                    return string.IsNullOrEmpty(strValue);
                }

                return strValue.Equals(b.strValue, comparison);
            }
            
            if (obj is string b1)
            {
                if (strValue == null)
                {
                    return b1 == "";
                }

                return strValue.Equals(b1, comparison);
            }
            return false;
        }
    }
}
