using Frosty.Core.Controls.Editors;
using FrostySdk.Ebx;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ConnectionPlugin.Editors
{
    public class EventConnectionEditor : FrostyTypeEditor<EventConnectionControl>
    {
        public EventConnectionEditor()
        {
            ValueProperty = PropertyConnectionControl.ValueProperty;
            NotifyOnTargetUpdated = true;
        }
    }
    public class EventConnectionControl : PropertyConnectionControl
    {
        static EventConnectionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EventConnectionControl), new FrameworkPropertyMetadata(typeof(EventConnectionControl)));
        }

        protected override void RefreshUI()
        {
            dynamic eventConnection = Value;
            StackPanel sp = GetTemplateChild("PART_StackPanel") as StackPanel;

            (sp.Children[0] as TextBlock).Text = GetEntity(eventConnection.Source);
            (sp.Children[4] as TextBlock).Text = GetEntity(eventConnection.Target);
            (sp.Children[2] as TextBlock).Text = Sanitize(eventConnection.Source, eventConnection.SourceEvent.Name);
            (sp.Children[6] as TextBlock).Text = Sanitize(eventConnection.Target, eventConnection.TargetEvent.Name);
        }

        protected override string Sanitize(PointerRef pr, string value)
        {
            if (value.StartsWith("0x"))
                value = value.Remove(0, 2);
            if (value == "00000000")
                value = "Self";
            return value;
        }
    }
}