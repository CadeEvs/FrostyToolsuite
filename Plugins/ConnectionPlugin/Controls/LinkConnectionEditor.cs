using Frosty.Core.Controls.Editors;
using FrostySdk.Ebx;
using Microsoft.CSharp.RuntimeBinder;
using System.Windows;
using System.Windows.Controls;

namespace ConnectionPlugin.Editors
{
    public class LinkConnectionEditor : FrostyTypeEditor<LinkConnectionControl>
    {
        public LinkConnectionEditor()
        {
            ValueProperty = PropertyConnectionControl.ValueProperty;
            NotifyOnTargetUpdated = true;
        }
    }
    public class LinkConnectionControl : PropertyConnectionControl
    {
        static LinkConnectionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkConnectionControl), new FrameworkPropertyMetadata(typeof(LinkConnectionControl)));
        }

        protected override void RefreshUI()
        {
            dynamic propConnection = Value;
            StackPanel sp = GetTemplateChild("PART_StackPanel") as StackPanel;

            (sp.Children[0] as TextBlock).Text = GetEntity(propConnection.Source);
            (sp.Children[4] as TextBlock).Text = GetEntity(propConnection.Target);
            (sp.Children[2] as TextBlock).Text = Sanitize(propConnection.Source, propConnection.SourceField);
            (sp.Children[6] as TextBlock).Text = Sanitize(propConnection.Target, propConnection.TargetField);
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