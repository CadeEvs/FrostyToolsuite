using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Editors
{
    public class LayersViewModel : Controls.IDockableItem
    {
        public string Header => "Layers";
        public string UniqueId => "UID_LevelEditor_Layers";
        public string Icon => "Images/Layers.png";
        public List<Layers.SceneLayer> Layers { get; private set; } = new List<Layers.SceneLayer>();

        private IEditorProvider owner;

        public LayersViewModel(IEditorProvider inOwner)
        {
            owner = inOwner;
            Layers.Add(owner.RootLayer);

            List<Layers.SceneLayer> layers = new List<Layers.SceneLayer>();
            owner.RootLayer.CollectLayers(layers);

            owner.RootLayer.SelectionChanged += SelectedLayerChanged;
            foreach (var layer in layers)
                layer.SelectionChanged += SelectedLayerChanged;
        }

        private void SelectedLayerChanged(object sender, Layers.LayerSelectionChangedEventArgs e)
        {
            if (e.Selected)
                owner.SelectLayer(e.Layer);
        }
    }
}
