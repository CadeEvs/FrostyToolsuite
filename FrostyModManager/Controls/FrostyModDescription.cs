using Frosty.Core.Mod;
using FrostySdk;
using FrostySdk.IO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrostyModManager.Controls
{
    public class ScreenshotButtonEventArgs : RoutedEventArgs
    {
        public ImageSource Screenshot { get; }

        public ScreenshotButtonEventArgs(ImageSource inSource)
        {
            Screenshot = inSource;
        }
    }

    public class AffectedFileInfo
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsAdded { get; }
        public bool IsModified { get; }

        public AffectedFileInfo(string n, string t, bool a, bool m)
        {
            Name = n;
            Type = t;
            IsAdded = a;
            IsModified = m;
        }
    }

    [TemplatePart(Name = PART_ScreenshotPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_ModIcon, Type = typeof(Image))]
    [TemplatePart(Name = PART_ModFilesListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_LoadingText, Type = typeof(TextBlock))]
    public class FrostyModDescription : Control
    {
        private const string PART_ScreenshotPanel = "PART_ScreenshotPanel";
        private const string PART_ModIcon = "PART_ModIcon";
        private const string PART_ModFilesListBox = "PART_ModFilesListBox";
        private const string PART_LoadingText = "PART_LoadingText";

        private StackPanel screenshotPanel;
        private Image modIcon;
        private ListBox modFilesListBox;
        private TextBlock loadingText;

        #region -- Properties --

        #region -- Mod --
        public static readonly DependencyProperty ModProperty = DependencyProperty.Register("Mod", typeof(IFrostyMod), typeof(FrostyModDescription), new FrameworkPropertyMetadata(null));
        public IFrostyMod Mod
        {
            get => (IFrostyMod)GetValue(ModProperty);
            set => SetValue(ModProperty, value);
        }
        #endregion

        #endregion

        public delegate void ScreenshotButtonEventHandler(object sender, ScreenshotButtonEventArgs e);

        private event ScreenshotButtonEventHandler screenshotClicked;
        public event ScreenshotButtonEventHandler ScreenshotClicked
        {
            add => screenshotClicked += value;
            remove => screenshotClicked -= value;
        }

        static FrostyModDescription()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyModDescription), new FrameworkPropertyMetadata(typeof(FrostyModDescription)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            screenshotPanel = GetTemplateChild(PART_ScreenshotPanel) as StackPanel;
            modIcon = GetTemplateChild(PART_ModIcon) as Image;
            modFilesListBox = GetTemplateChild(PART_ModFilesListBox) as ListBox;
            loadingText = GetTemplateChild(PART_LoadingText) as TextBlock;

            Loaded += FrostyModDescription_Loaded;
        }

        private async void FrostyModDescription_Loaded(object sender, RoutedEventArgs e)
        {
            if (Mod != null)
            {
                // icon
                if (Mod.ModDetails.Icon != null)
                    modIcon.Visibility = Visibility.Visible;

                // screenshots
                int idx = 0;
                foreach (ImageSource screenshot in Mod.ModDetails.Screenshots)
                {
                    Image screenshotImage = screenshotPanel.Children[idx++] as Image;

                    screenshotImage.Source = screenshot;
                    screenshotImage.Visibility = Visibility.Visible;
                    screenshotImage.MouseDown += (o, e2) =>
                    {
                        if (e2.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                            return;

                        Image img = o as Image;
                        screenshotClicked?.Invoke(this, new ScreenshotButtonEventArgs(img.Source));
                    };
                }

                List<AffectedFileInfo> affectedFiles = new List<AffectedFileInfo>();
                FrostyMod[] localMods;
                if (Mod is FrostyModCollection)
                {
                    localMods = (Mod as FrostyModCollection).Mods.ToArray();
                }
                else
                {
                    localMods = new FrostyMod[1];
                    localMods[0] = Mod as FrostyMod;
                }

                foreach (FrostyMod localMod in localMods)
                {
                    if (localMod != null)
                    {
                        await Task.Run(() =>
                        {
                            if (localMod.NewFormat)
                            {
                                foreach (BaseModResource resource in localMod.Resources)
                                {
                                    if (resource.Type == ModResourceType.Embedded)
                                        continue;

                                    string resType = resource.Type.ToString().ToUpper();
                                    string resourceName = resource.Name;

                                    if (resource.UserData != "")
                                    {
                                        string[] arr = resource.UserData.Split(';');
                                        resType = arr[0].ToUpper();
                                        resourceName = arr[1];
                                    }
                                    affectedFiles.Add(new AffectedFileInfo(resourceName, resType, resource.IsAdded, resource.IsModified));
                                }
                            }
                            else
                            {
                                // affected files
                                DbObject modObj = null;
                                using (DbReader reader = new DbReader(new FileStream(localMod.Path, FileMode.Open, FileAccess.Read), null))
                                    modObj = reader.ReadDbObject();

                                int resourceId = -1;
                                foreach (DbObject resource in modObj.GetValue<DbObject>("resources"))
                                {
                                    resourceId++;

                                    string resourceType = resource.GetValue<string>("type");
                                    if (resourceType == "embedded")
                                        continue;

                                    string name = resource.GetValue<string>("name");

                                    bool isModify = false;
                                    bool isAdded = false;

                                    foreach (DbObject action in modObj.GetValue<DbObject>("actions"))
                                    {
                                        if (action.GetValue<int>("resourceId") == resourceId)
                                        {
                                            string type = action.GetValue<string>("type");
                                            switch (type)
                                            {
                                                case "modify": isModify = true; break;
                                                case "add": isAdded = true; break;
                                            }
                                        }
                                    }

                                    affectedFiles.Add(new AffectedFileInfo(name, resource.GetValue<string>("type").ToUpper(), isAdded, isModify));
                                }
                            }
                        });
                    }
                }

                loadingText.Visibility = Visibility.Collapsed;
                modFilesListBox.ItemsSource = affectedFiles;
            }
        }
    }
}