using System.Windows.Controls;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyBooleanEditor : FrostyTypeEditor<CheckBox>
    {
        public FrostyBooleanEditor()
        {
            ValueProperty = CheckBox.IsCheckedProperty;
        }
    }
    public class FrostyAutoDisableBooleanEditor : FrostyTypeEditor<CheckBox>
    {
        public FrostyAutoDisableBooleanEditor()
        {
            ValueProperty = CheckBox.IsCheckedProperty;
        }

        protected override void CustomizeEditor(CheckBox editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Checked += Editor_Checked;
            editor.Loaded += Editor_Loaded;
        }

        private void Editor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
                cb.IsEnabled = false;
        }

        private void Editor_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            cb.IsEnabled = false;
        }
    }
}
