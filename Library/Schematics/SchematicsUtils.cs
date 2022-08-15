namespace LevelEditorPlugin.Library.Schematics
{
    public static class SchematicsUtils
    {
        public static int HashString(string value)
        {
            if (value.StartsWith("0x"))
            {
                return int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            return Frosty.Hash.Fnv1.HashString(value);
        }
    }
}