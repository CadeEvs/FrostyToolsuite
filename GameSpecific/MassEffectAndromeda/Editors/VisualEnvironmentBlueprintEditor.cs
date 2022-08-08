using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Editors
{
    public class VisualEnvironmentBlueprintEditor : LogicEditor
    {
        public VisualEnvironmentBlueprintEditor(ILogger inLogger)
            : base(inLogger)
        {
            viewModel.DockManager.LoadFromConfig("VisualEnvironmentBlueprintEditor", new Controls.DockManager.DockManagerConfigData()
            {
                Layouts = new List<Controls.DockManager.DockLayoutData>()
                    {
                        new Controls.DockManager.DockLayoutData()
                        {
                            UniqueId = "UID_LevelEditor_Properties",
                            IsVisible = true,
                            IsSelected = true,
                            Location = Controls.DockLocation.TopRight
                        }
                    }
            });
        }

        //public override List<ToolbarItem> RegisterToolbarItems()
        //{
        //    var items = base.RegisterToolbarItems();
        //    items.Add(new FloatingOnlyDockingToolbarItem("", "Show/Hide timeline editor", "Images/Timeline.png", new RelayCommand((o) => viewModel.DockManager.AddItem(((DockingToolbarItem)o).Location, new TimelineViewModel(this))), viewModel.DockManager, "UID_LevelEditor_Timeline"));
        //    return items;
        //}

        protected override void Initialize()
        {
            FrostySdk.Ebx.VisualEnvironmentReferenceObjectData objectData = new FrostySdk.Ebx.VisualEnvironmentReferenceObjectData()
            {
                Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = Asset.FileGuid, ClassGuid = Asset.RootInstanceGuid })
            };

            entity = Entities.Entity.CreateEntity(objectData, null) as Entities.VisualEnvironmentReferenceObject;
            rootLayer = MakeFakeLayer();

            viewModel.DockManager.AddItemOnLoad(new PropertiesViewModel(viewModel));

            base.Initialize();
        }

        public override void Closed()
        {
            viewModel.DockManager.SaveToConfig("VisualEnvironmentBlueprintEditor");
            base.Closed();
        }
    }
}
