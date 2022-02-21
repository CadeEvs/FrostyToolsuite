namespace LuaPlugin.CodeEditor
{
    public class CodeLocation
    {
        public int Index { get; }
        public int Line { get; }
        public int Column { get; }

        public CodeLocation(int index, int line, int column)
        {
            Index = index;
            Line = line;
            Column = column;
        }
    }
}
