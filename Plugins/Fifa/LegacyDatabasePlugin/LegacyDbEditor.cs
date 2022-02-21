using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Legacy;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using LegacyDatabasePlugin.Converters;
using LegacyDatabasePlugin.Database;
using LegacyDatabasePlugin.IO;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace LegacyDatabasePlugin
{
    [TemplatePart(Name = PART_TableView, Type = typeof(ListView))]
    [TemplatePart(Name = PART_TableList, Type = typeof(ListBox))]
    public class LegacyDbEditor : FrostyAssetEditor
    {
        private const string PART_TableView = "PART_TableView";
        private const string PART_TableList = "PART_TableList";

        private const int MinColumnWidth = 50;
        private const int MaxColumnWidth = 400;

        private ListView tableView;
        private ListBox tableList;

        private LegacyDb database;
        private int currentTableIndex;
        private bool firstTimeLoad = true;

        static LegacyDbEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegacyDbEditor), new FrameworkPropertyMetadata(typeof(LegacyDbEditor)));
        }

        public LegacyDbEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tableView = GetTemplateChild(PART_TableView) as ListView;
            tableList = GetTemplateChild(PART_TableList) as ListBox;

            Loaded += LegacyDbEditor_Loaded;
            tableList.SelectionChanged += TableList_SelectionChanged;
            tableView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>();
        }

        private void TableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LegacyDbTable table = tableList.SelectedItem as LegacyDbTable;
            if (table == null)
                return;

            currentTableIndex = tableList.SelectedIndex;
            tableView.ItemsSource = null;

            GridView gridView = tableView.View as GridView;
            gridView.Columns.Clear();

            foreach (LegacyDbColumn column in table.Columns)
            {
                GridViewColumn gvColumn = new GridViewColumn()
                {
                    HeaderTemplate = Template.Resources["gvColumnTemplate"] as DataTemplate,
                    Header = column,
                };
                Binding b = new Binding(".")
                {
                    Converter = new ColumnRowConverter(),
                    ConverterParameter = column.Name
                };
                gvColumn.DisplayMemberBinding = b;
                gridView.Columns.Add(gvColumn);
            }

            tableView.ItemsSource = table.Rows;
        }

        private void LegacyDbEditor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                AssetEntry entry = AssetEntry;
                FrostyTaskWindow.Show("Loading Database", "", (task) =>
                {
                    string metaName = entry.Name.Replace(".db", "-meta.xml");
                    LegacyFileEntry metaEntry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", metaName);
                    Stream metaStream = null;

                    if (metaEntry == null)
                    {
                        string filename = "Resources/Meta/" + entry.Filename + "-meta.xml";
                        if (File.Exists(filename))
                            metaStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        metaStream = App.AssetManager.GetCustomAsset("legacy", metaEntry);
                    }

                    using (LegacyDbReader reader = new LegacyDbReader(metaStream, App.AssetManager.GetCustomAsset("legacy", entry)))
                        database = reader.ReadDb();

                    metaStream?.Dispose();
                });
                firstTimeLoad = false;
            }

            tableList.ItemsSource = database.Tables;
            tableList.SelectedIndex = currentTableIndex;
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.OriginalSource is Thumb senderAsThumb)
            {
                if (!(senderAsThumb.TemplatedParent is GridViewColumnHeader header))
                    return;

                if (header.Column.ActualWidth < MinColumnWidth)
                    header.Column.Width = MinColumnWidth;
            }

            //if (header.Column.ActualWidth > MaxColumnWidth)
            //    header.Column.Width = MaxColumnWidth;
        }
    }
}
