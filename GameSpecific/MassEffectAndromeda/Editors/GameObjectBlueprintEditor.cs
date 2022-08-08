using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Editors
{
    public class GameObjectBlueprintEditor : LogicEditor
    {
        public GameObjectBlueprintEditor(ILogger inLogger)
            : base(inLogger)
        {
            viewModel.DockManager.LoadFromConfig("GameObjectBlueprintEditor", new Controls.DockManager.DockManagerConfigData()
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
            FrostySdk.Ebx.ObjectReferenceObjectData objectData = new FrostySdk.Ebx.ObjectReferenceObjectData()
            {
                Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = Asset.FileGuid, ClassGuid = Asset.RootInstanceGuid })
            };

            entity = Entities.Entity.CreateEntity(objectData, null) as Entities.ObjectReferenceObject;
            rootLayer = MakeFakeLayer();

            viewModel.DockManager.AddItemOnLoad(new PropertiesViewModel(viewModel));

            base.Initialize();
        }

        public override void Closed()
        {
            viewModel.DockManager.SaveToConfig("GameObjectBlueprintEditor");
            base.Closed();
        }
    }
}
