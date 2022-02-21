using Frosty.Controls;
using FrostySdk;
using System.Globalization;
using System.Windows;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyLocalizedStringHashEditor : FrostyTypeEditor<FrostyLocalizedStringHashControl>
    {
        public FrostyLocalizedStringHashEditor()
        {
            ValueProperty = FrostyLocalizedStringHashControl.ValueProperty;
        }
    }

    public class FrostyLocalizedStringReferenceEditor : FrostyTypeEditor<FrostyLocalizedStringReferenceControl>
    {
        public FrostyLocalizedStringReferenceEditor()
        {
            ValueProperty = FrostyLocalizedStringReferenceControl.ValueProperty;
        }
    }

    public class FrostyLocalizedStringReferenceControl : FrostyEllipsedTextBox
    {
        #region -- Properties --

        #region -- Value --
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrostyLocalizedStringReferenceControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotFocus += FrostyLocalizedStringReferenceControl_GotFocus;
            GotKeyboardFocus += FrostyLocalizedStringReferenceControl_GotFocus;
            LostFocus += FrostyLocalizedStringReferenceControl_LostFocus;
            LostKeyboardFocus += FrostyLocalizedStringReferenceControl_LostFocus;

            ShowStringDisplay(((dynamic)Value).StringId);
        }

        private void FrostyLocalizedStringReferenceControl_LostFocus(object sender, RoutedEventArgs e)
        {
            int stringId = ((dynamic)Value).StringId;

            // check for change
            if (!int.TryParse(Text, NumberStyles.HexNumber, null, out int newStringId))
                newStringId = stringId;

            if (newStringId != stringId)
            {
                // update
                dynamic stringRef = TypeLibrary.CreateObject("LocalizedStringReference");
                stringRef.StringId = newStringId;
                Value = stringRef;
            }

            ShowStringDisplay(stringId);
        }

        private void FrostyLocalizedStringReferenceControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Text = ((dynamic)Value).StringId.ToString("X8");
            ToolTip = null;
            SelectAll();
        }

        private void ShowStringDisplay(int stringId)
        {
            Text = LocalizedStringDatabase.Current.GetString((uint)stringId);
            ToolTip = stringId.ToString("X8");
        }
    }

    public class FrostyLocalizedStringHashControl : FrostyEllipsedTextBox
    {
        #region -- Properties --

        #region -- Value --
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrostyLocalizedStringHashControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            GotFocus += FrostyLocalizedStringIdControl_GotFocus;
            GotKeyboardFocus += FrostyLocalizedStringIdControl_GotFocus;
            LostFocus += FrostyLocalizedStringIdControl_LostFocus;
            LostKeyboardFocus += FrostyLocalizedStringIdControl_LostFocus;

            ShowStringDisplay(((dynamic)Value));
        }

        private void FrostyLocalizedStringIdControl_LostFocus(object sender, RoutedEventArgs e)
        {
            int stringId = ((dynamic)Value);

            int newStringId = 0;
            if (Text.StartsWith("ID_"))
            {
                newStringId = (int)HashStringId(Text);
            }
            else
            {
                // check for change
                if (!int.TryParse(Text, NumberStyles.HexNumber, null, out newStringId))
                    newStringId = stringId;
            }

            if (newStringId != stringId)
            {
                // update
                Value = newStringId;
            }

            ShowStringDisplay(stringId);
        }

        private uint HashStringId(string stringId)
        {
            uint result = 0xFFFFFFFF;
            for (int i = 0; i < stringId.Length; i++)
                result = stringId[i] + 33 * result;
            return result;
        }

        private void FrostyLocalizedStringIdControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Text = ((dynamic)Value).ToString("X8");
            ToolTip = null;
            SelectAll();
        }

        private void ShowStringDisplay(int stringId)
        {
            Text = LocalizedStringDatabase.Current.GetString((uint)stringId);
            ToolTip = stringId.ToString("X8");
        }
    }
}
