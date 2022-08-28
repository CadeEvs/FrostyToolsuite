using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Layers;
using LevelEditorPlugin.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LevelEditorPlugin.Editors
{
    public class LogicEditor : ToolbarAssetEditor, IEditorProvider
    {
        public SchematicsViewModel ViewModel => viewModel;
        public LevelEditorScreen Screen => null;

        public SceneLayer RootLayer => rootLayer;

#pragma warning disable CS0067
        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;
        public event EventHandler<SelectedLayerChangedEventArgs> SelectedLayerChanged;
#pragma warning restore CS0067

        protected SchematicsViewModel viewModel;
        protected ReferenceObject entity;
        protected Layers.SceneLayer rootLayer;
        protected EntityWorld world;

        static LogicEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogicEditor), new FrameworkPropertyMetadata(typeof(LogicEditor)));
        }

        public LogicEditor(ILogger inLogger)
            : base(inLogger)
        {
            viewModel = new SchematicsViewModel(this, null, true);
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            List<ToolbarItem> toolbarItems = base.RegisterToolbarItems();
            toolbarItems.AddRange(viewModel.RegisterToolbarItems());
            
            return toolbarItems;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PerformTemplateMagic();
        }

        public void CenterOnSelection()
        {
            throw new NotImplementedException();
        }

        public void SelectEntity(Entity newSelection)
        {
        }

        public void SelectLayer(SceneLayer newSelection)
        {
        }

        protected override void Initialize()
        {
            viewModel.SetLayer(rootLayer);
        }

        public override void Closed()
        {
            viewModel.Unload(null);
            entity.Destroy();
            base.Closed();
        }

        // @temp
        protected Layers.SceneLayer MakeFakeLayer()
        {
            string layerName = Path.GetFileName(entity.Blueprint.Name);
            Layers.SceneLayer layer = new Layers.SceneLayer(entity, layerName, new SharpDX.Color(0.0f, 0.5f, 0.0f, 1.0f));

            List<Entity> entities = (List<Entity>)entity.GetType().GetField("entities", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(entity);
            foreach (Entity entity in entities)
            {
                if (entity is ILayerEntity)
                {
                    ILayerEntity entityLayer = entity as ILayerEntity;
                    Layers.SceneLayer childLayer = entityLayer.GetLayer();
                    if (childLayer != null)
                        layer.ChildLayers.Add(childLayer);
                }
                else
                {
                    layer.AddEntity(entity);
                    entity.SetOwner(entity);
                }
            }

            return layer;
        }
    }
}
