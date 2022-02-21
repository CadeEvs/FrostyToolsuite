using Frosty.Core.Controls.Editors;
using FrostySdk.Ebx;
using System.Windows;

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