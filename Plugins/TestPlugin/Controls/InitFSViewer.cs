using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.IO;
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
    [TemplatePart(Name = PART_RevertMenuItem, Type = typeof(MenuItem))]
    public class InitFSViewer : FrostyBaseEditor
    {
        private const string PART_ListBox = "PART_ListBox";
        private const string PART_Contents = "PART_Contents";
        private const string PART_DataExplorer = "PART_DataExplorer";
        private const string PART_RevertMenuItem = "PART_RevertMenuItem";

        private ListBox listBox;
        private TextBox contentsBox;
        private FrostyDataExplorer dataExplorer;
        private MenuItem revertMenuItem;

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
            contentsBox.TextChanged += ContentsBox_TextChanged;
            dataExplorer = GetTemplateChild(PART_DataExplorer) as FrostyDataExplorer;
            revertMenuItem = GetTemplateChild(PART_RevertMenuItem) as MenuItem;

            dataExplorer.SelectionChanged += DataExplorer_SelectionChanged;
            listBox.SelectionChanged += ListBox_SelectionChanged;
            revertMenuItem.Click += RevertMenuItem_Click;

            Loaded += InitFSViewer_Loaded;
        }

        private void RevertMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FsFileEntry entry = dataExplorer.SelectedAsset as FsFileEntry;
            App.AssetManager.RevertAsset(entry);
        }

        private void ContentsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FsFileEntry entry = dataExplorer.SelectedAsset as FsFileEntry;

            Stream stream = App.AssetManager.GetCustomAsset("fs", entry);

            string originalValue = "";
            if (stream != null)
            {
                using (TextReader reader = new StreamReader(stream))
                    originalValue = reader.ReadToEnd();
            }

            if (!contentsBox.Text.Equals(originalValue))
            {
                DbObject file = new DbObject();
                file.AddValue("name", entry.Name);

                MemoryStream ms = new MemoryStream();
                using (TextWriter writer = new StreamWriter(ms))
                    writer.Write(contentsBox.Text);
                file.AddValue("payload", ms.ToArray());
                ms.Dispose();

                DbObject fileStub = new DbObject();
                fileStub.AddValue("$file", file);

                using (DbWriter writer = new DbWriter(new MemoryStream()))
                {
                    writer.Write(fileStub);
                    App.AssetManager.ModifyCustomAsset("fs", entry.Name, writer.ToByteArray());
                }
            }

            dataExplorer.RefreshItems();
        }

        private void DataExplorer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            FsFileEntry entry = dataExplorer.SelectedAsset as FsFileEntry;

            Stream ms = App.AssetManager.GetCustomAsset("fs", entry);

            if (ms != null)
            {
                using (TextReader reader = new StreamReader(ms))
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
