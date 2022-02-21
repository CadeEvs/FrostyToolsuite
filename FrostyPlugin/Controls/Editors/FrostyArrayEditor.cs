using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Frosty.Core.Controls.Editors
{
    [TemplatePart(Name = "PART_RemoveButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    public class FrostyArrayControl : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(FrostyArrayControl), new FrameworkPropertyMetadata(0));
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        static FrostyArrayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyArrayControl), new FrameworkPropertyMetadata(typeof(FrostyArrayControl)));
        }

        public FrostyPropertyGridItemData Item { get; set; }

        private Button removeButton;
        private Button addButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            removeButton = GetTemplateChild("PART_RemoveButton") as Button;
            addButton = GetTemplateChild("PART_AddButton") as Button;

            removeButton.Click += RemoveButton_Click;
            addButton.Click += AddButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            IList list = (IList)Item.Value;
            Type type = list.GetType();

            Type genericType = type.GetGenericArguments()[0];

            object value = Activator.CreateInstance(genericType);
            object defValue = Activator.CreateInstance(genericType);

            Item.AddChild(value, defValue);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Item.ClearChildren();
        }
    }
    public class FrostyArrayEditor : FrostyTypeEditor<FrostyArrayControl>
    {
        public FrostyArrayEditor()
        {
            ValuePath = "Value.Count";
            ValueProperty = FrostyArrayControl.ValueProperty;
            BindingMode = BindingMode.OneWay;
        }

        protected override void CustomizeEditor(FrostyArrayControl editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Item = item;
        }
    }
}
