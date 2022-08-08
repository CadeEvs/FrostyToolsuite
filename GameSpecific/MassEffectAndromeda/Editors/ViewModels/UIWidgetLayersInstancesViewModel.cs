using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Editors
{
    public class UIWidgetLayersInstancesViewModel : Controls.IDockableItem
    {
        public string Header => "Instances";
        public string UniqueId => "UID_LevelEditor_WidgetLayers";
        public string Icon => "Images/Layers.png";

        public UIWidgetLayersInstancesViewModel(IEditorProvider inOwner)
        {
        }
    }
}
