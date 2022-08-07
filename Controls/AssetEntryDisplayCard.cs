using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Bookmarks;
using FrostySdk.Managers;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LevelEditorPlugin.Controls
{
    public class AssetEntryDisplayCard : Control
    {
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(SvgImageData), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ThumbnailSourceProperty = DependencyProperty.Register("ThumbnailSource", typeof(ImageSource), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty AssetTypeProperty = DependencyProperty.Register("AssetType", typeof(string), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty IsBookmarkedProperty = DependencyProperty.Register("IsBookmarked", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasLayoutProperty = DependencyProperty.Register("HasLayout", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasThumbnailProperty = DependencyProperty.Register("HasThumbnail", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register("IsModified", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));

        public SvgImageData IconData
        {
            get => (SvgImageData)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }
        public ImageSource ThumbnailSource
        {
            get => (ImageSource)GetValue(ThumbnailSourceProperty);
            set => SetValue(ThumbnailSourceProperty, value);
        }
        public string AssetType
        {
            get => (string)GetValue(AssetTypeProperty);
            set => SetValue(AssetTypeProperty, value);
        }
        public bool IsBookmarked
        {
            get => (bool)GetValue(IsBookmarkedProperty);
            set => SetValue(IsBookmarkedProperty, value);
        }
        public bool HasLayout
        {
            get => (bool)GetValue(HasLayoutProperty);
            set => SetValue(HasLayoutProperty, value);
        }
        public bool HasThumbnail
        {
            get => (bool)GetValue(HasThumbnailProperty);
            set => SetValue(HasThumbnailProperty, value);
        }
        public bool IsModified
        {
            get => (bool)GetValue(IsModifiedProperty);
            set => SetValue(IsModifiedProperty, value);
        }
        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        static AssetEntryDisplayCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AssetEntryDisplayCard), new FrameworkPropertyMetadata(typeof(AssetEntryDisplayCard)));
        }

        public AssetEntryDisplayCard(EbxAssetEntry entry, SvgImageData icon)
        {
            IconData = icon;
            AssetType = entry.Type;
            IsBookmarked = IsInBookmarkDb(entry, BookmarkDb.CurrentContext.Bookmarks);
            HasLayout = SchematicsLayoutManager.Instance.GetLayout(entry.Guid) != null;
            IsModified = entry.IsModified;
            IsDirty = entry.IsDirty;

            string thumbnailPath = $"{App.ProfileSettingsPath}/Thumbnails/{entry.Guid}.png";
            if (File.Exists(thumbnailPath))
            {
                HasThumbnail = true;
                ThumbnailSource = new ImageSourceConverter().ConvertFromString(thumbnailPath) as ImageSource;
            }
        }

        public ImageSource GetImageSource(double width, double height)
        {
            Grid grid = new Grid()
            {
                Width = width,
                Height = height
            };
            grid.Children.Add(this);
            grid.Measure(new Size(width, height));
            grid.Arrange(new Rect(0, 0, width, height));

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)width, (int)height, 96.0, 96.0, PixelFormats.Pbgra32);
            renderTarget.Render(grid);
            renderTarget.Freeze();

            return renderTarget;
        }

        private bool IsInBookmarkDb(EbxAssetEntry entry, List<BookmarkItem> bookmarkItems)
        {
            bool retVal = false;
            foreach (BookmarkItem item in bookmarkItems)
            {
                if (item.IsFolder)
                {
                    retVal = IsInBookmarkDb(entry, item.Children);
                    if (retVal)
                        return retVal;
                }
                else
                {
                    if (item.Target is AssetBookmarkTarget)
                    {
                        AssetBookmarkTarget assetTarget = item.Target as AssetBookmarkTarget;
                        if (assetTarget.Asset == entry)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
