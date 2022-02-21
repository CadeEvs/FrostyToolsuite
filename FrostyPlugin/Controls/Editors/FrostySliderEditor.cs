using FrostySdk.Attributes;
using System.Windows;
using System.Windows.Controls;

namespace Frosty.Core.Controls.Editors
{
    public class FrostySliderControl : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(FrostySliderControl), new FrameworkPropertyMetadata(0.0f));
        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public float Minimum { get; set; } = 0.0f;
        public float Maximum { get; set; } = 1.0f;
        public float SmallChange { get; set; } = 0.01f;
        public float LargeChange { get; set; } = 0.1f;
        public bool IsSnapToTickEnabled { get; set; }

        static FrostySliderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostySliderControl), new FrameworkPropertyMetadata(typeof(FrostySliderControl)));
        }
    }

    public class FrostySliderEditor : FrostyTypeEditor<FrostySliderControl>
    {
        public FrostySliderEditor()
        {
            ValueProperty = FrostySliderControl.ValueProperty;
        }

        protected override void CustomizeEditor(FrostySliderControl editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);

            SliderMinMaxAttribute attr = item.GetMetaDataAttribute<SliderMinMaxAttribute>();
            if (attr != null)
            {
                editor.Minimum = attr.MinValue;
                editor.Maximum = attr.MaxValue;
                editor.SmallChange = attr.SmallChange;
                editor.LargeChange = attr.LargeChange;
                editor.IsSnapToTickEnabled = attr.IsSnapToTickEnabled;
            }
        }
    }
}
