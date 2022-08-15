using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LevelEditorPlugin.Editors
{
    public class UIWidgetDropData
    {
        public Type DataType;
    }

    public class UIWidgetToolboxViewModel : SchematicsToolboxViewModel
    {
        public override string UniqueId => "UID_LevelEditor_WidgetToolbox";

        public override bool InitiateDrag(out object dataToDrag, out FrameworkElement optionalVisual)
        {
            dataToDrag = null;
            optionalVisual = null;

            if (m_selectedType == null)
            {
                return false;
            }

            dataToDrag = new UIWidgetDropData() { DataType = m_selectedType.Type };
            EntityBindingAttribute attr = m_selectedType.Type.GetCustomAttribute<EntityBindingAttribute>();

            optionalVisual = new Rectangle() { Width = 50, Height = 50, Fill = Brushes.White, Stroke = Brushes.Black, StrokeThickness = 1.0 };

            return true;
        }

        protected override bool IsTypeValid(Type incomingType)
        {
            return incomingType.GetCustomAttribute<EntityBindingAttribute>() != null && incomingType.IsSubclassOf(typeof(UIElementEntity));
        }
    }
}
