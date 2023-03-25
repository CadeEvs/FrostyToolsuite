using DifficultyWeaponTableDataPlugin.Resources;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DifficultyWeaponTableDataPlugin.Controls
{
    // Classes that derive from FrostyAssetEditor are used to edit assets specifically, they have a lot of boiler plate
    // code for loading assets and their dependencies.

    // This editor is used to edit assets of type DifficultyWeaponTableData, it is instantiated from the GetEditor
    // function of the corresponding AssetDefinition
    [TemplatePart(Name = "PART_AssetPropertyGrid", Type = typeof(FrostyPropertyGrid))]
    public class DifficultyWeaponTableEditor : FrostyAssetEditor
    {
        private FrostyPropertyGrid propertyGrid;

        static DifficultyWeaponTableEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DifficultyWeaponTableEditor), new FrameworkPropertyMetadata(typeof(DifficultyWeaponTableEditor)));
        }

        public DifficultyWeaponTableEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            propertyGrid = GetTemplateChild("PART_AssetPropertyGrid") as FrostyPropertyGrid;
            propertyGrid.OnModified += PropertyGrid_OnModified;
        }

        // This function is used to override the editors load function to ensure that the asset is loaded
        // with a specific subclass of EbxAsset
        protected override EbxAsset LoadAsset(EbxAssetEntry entry)
        {
            DifficultyWeaponTableData loadedAsset = App.AssetManager.GetEbxAs<DifficultyWeaponTableData>(entry);
            return loadedAsset;
        }

        // Functionality to perform when a property in the property grid is modified, in this case
        // the modified binding has been removed, as the editor will handle the modified data specifically
        // and then invoke the OnAssetModified function manually
        private void PropertyGrid_OnModified(object sender, ItemModifiedEventArgs e)
        {
            // obtain the row and column of the edit via the item property paths
            string colIndex = e.Item.Parent.Name.Trim('[', ']');
            string rowIndex = e.Item.Parent.Parent.Parent.Name.Trim('[', ']');

            // add or modify the value on the asset
            DifficultyWeaponTableData assetData = asset as DifficultyWeaponTableData;
            assetData.ModifyValue(int.Parse(rowIndex), int.Parse(colIndex), (float)e.NewValue);

            // invoke the OnAssetModified function
            App.AssetManager.ModifyEbx(AssetEntry.Name, assetData);
            InvokeOnAssetModified();
        }
    }
}
