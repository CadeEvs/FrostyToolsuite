using System.Collections.Generic;

namespace Frosty.Core.Misc
{
    public class CustomComboData<T, U>
    {
        public T SelectedValue => Values[SelectedIndex];
        public U SelectedName => Names[SelectedIndex];

        public List<T> Values { get; set; }
        public List<U> Names { get; set; }
        public int SelectedIndex { get; set; }

        public CustomComboData(List<T> InValues, List<U> InNames)
        {
            Values = InValues;
            Names = InNames;
            SelectedIndex = 0;
        }
    }
}
