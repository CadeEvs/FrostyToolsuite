using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditorPlugin.Editors
{
    public class TerrainLayer : INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public bool SelectedHoleMaskPreview 
        {
            get => selectedHoleMaskPreview;
            set
            {
                if (selectedHoleMaskPreview != value)
                {
                    selectedHoleMaskPreview = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool selectedHoleMaskPreview;

        public TerrainLayer(FrostySdk.Ebx.TerrainLayerData terrainLayerData, int layerIndex)
        {
            Name = terrainLayerData.__Id;
            Index = layerIndex;

            selectedHoleMaskPreview = false;
        }

        public TerrainLayer(string name, int layerIndex)
        {
            Name = name;
            Index = layerIndex;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class TerrainLayersViewModel : Controls.IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Terrain Layers";
        public string UniqueId => "UID_LevelEditor_TerrainLayers";
        public string Icon => "Images/Terrain.png";
        public ObservableCollection<TerrainLayer> TerrainLayers => terrainLayers;
        public int SelectedLayerIndex
        {
            get => selectedLayerIndex;
            set
            {
                if (selectedLayerIndex != value)
                {
                    selectedLayerIndex = value;
                    terrainEntity.PreviewLayerIndex = selectedLayerIndex;
                    NotifyPropertyChanged();
                }
            }
        }
        public int SelectedHoleIndex
        {
            get => selectedHoleIndex;
            set
            {
                if (selectedHoleIndex != value)
                {
                    selectedHoleIndex = value;
                    terrainEntity.PreviewHoleIndex = selectedHoleIndex;
                    NotifyPropertyChanged();
                }
                else if (selectedHoleIndex == value && selectedHoleIndex != -1)
                {
                    selectedHoleIndex = -1;
                    terrainEntity.PreviewHoleIndex = selectedHoleIndex;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICommand LayerHoleMaskClickCommand { get; private set; }

        private LevelEditor owner;
        private Entities.TerrainEntity terrainEntity;
        private ObservableCollection<TerrainLayer> terrainLayers = new ObservableCollection<TerrainLayer>();
        private int selectedLayerIndex;
        private int selectedHoleIndex;

        public TerrainLayersViewModel(LevelEditor inOwner)
        {
            owner = inOwner;
            selectedHoleIndex = -1;
            selectedLayerIndex = 0;

            List<Entities.Entity> entities = new List<Entities.Entity>();
            owner.RootLayer.CollectEntities(entities);

            terrainEntity = entities.Find(e => e is Entities.TerrainEntity) as Entities.TerrainEntity;
            if (terrainEntity != null)
            {
                for (int i = 0; i < terrainEntity.Terrain.Data.TerrainLayers.Count; i++)
                {
                    var layer = terrainEntity.Terrain.Data.TerrainLayers[i];
                    terrainLayers.Add(new TerrainLayer(layer.GetObjectAs<FrostySdk.Ebx.TerrainLayerData>(), i));
                }

                if (terrainEntity.Terrain.MaxLayerCount > terrainLayers.Count)
                {
                    // appears to be a hole mask every time
                    terrainLayers.Add(new TerrainLayer("Hole Mask", terrainLayers.Count));
                }
            }

            LayerHoleMaskClickCommand = new RelayCommand((o) =>
            {
                if (SelectedHoleIndex != -1)
                    terrainLayers[SelectedHoleIndex].SelectedHoleMaskPreview = false;

                var layer = o as TerrainLayer;
                if (SelectedHoleIndex == layer.Index)
                {
                    SelectedHoleIndex = -1;
                    layer.SelectedHoleMaskPreview = false;
                }
                else
                {
                    SelectedHoleIndex = layer.Index;
                    layer.SelectedHoleMaskPreview = true;
                }
            });
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
