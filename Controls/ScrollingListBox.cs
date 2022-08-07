using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LevelEditorPlugin.Controls
{
    public class ScrollingListBox : ListBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetResourceReference(StyleProperty, typeof(ListBox));
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ScrollIntoView(e.AddedItems[0]);
            }

            base.OnSelectionChanged(e);
        }
    }
}
