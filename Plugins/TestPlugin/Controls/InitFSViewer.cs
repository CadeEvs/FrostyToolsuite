using Frosty.Core;
using Frosty.Core.Controls;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestPlugin.Managers;

namespace TestPlugin.Controls
{
    // Classes that derive from FrostyBaseEditor are used for generic editors, they contain no boiler plate code for loading of any kind of
    // data, it is left up to the developer to decide on what the editor will do and how it will function.

    [TemplatePart(Name = PART_ListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_Contents, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_DataExplorer, Type = typeof(FrostyDataExplorer))]
    public class InitFSViewer : FrostyBaseEditor
    {
        private const string PART_ListBox = "PART_ListBox";
        private const string PART_Contents = "PART_Contents";
        private const string PART_DataExplorer = "PART_DataExplorer";

        private ListBox listBox;
        private TextBox contentsBox;
        private FrostyDataExplorer dataExplorer;

        private List<FsFileEntry> entries;

        private bool firstLoad = true;

        static InitFSViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InitFSViewer), new FrameworkPropertyMetadata(typeof(InitFSViewer)));
        }

        public InitFSViewer()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            listBox = GetTemplateChild(PART_ListBox) as ListBox;
            contentsBox = GetTemplateChild(PART_Contents) as TextBox;
            dataExplorer = GetTemplateChild(PART_DataExplorer) as FrostyDataExplorer;

            dataExplorer.SelectionChanged += DataExplorer_SelectionChanged;
            listBox.SelectionChanged += ListBox_SelectionChanged;

            Loaded += InitFSViewer_Loaded;
        }

        private void DataExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            FsFileEntry entry = dataExplorer.SelectedAsset as FsFileEntry;

            if (entry != null)
            {
                byte[] buf = App.FileSystem.GetFileFromMemoryFs(entry.Name);

                using (TextReader reader = new StreamReader(new MemoryStream(buf)))
                    contentsBox.Text = reader.ReadToEnd();
            }
            else
            {
                contentsBox.Text = "";
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filename = listBox.SelectedItem as string;
            byte[] buf = App.FileSystem.GetFileFromMemoryFs(filename);

            using (TextReader reader = new StreamReader(new MemoryStream(buf)))
                contentsBox.Text = reader.ReadToEnd();
        }

        private void InitFSViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                //listBox.ItemsSource = App.FileSystem.EnumerateFilesInMemoryFs();

                dataExplorer.ItemsSource = App.AssetManager.EnumerateCustomAssets("fs");

                firstLoad = false;
            }
        }
    }
}
