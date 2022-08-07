using Frosty.Core;
using Frosty.Core.Controls;
using LevelEditorPlugin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditorPlugin.Editors
{
    public class IconCache
    {
        #region -- Singleton --

        public static IconCache Instance { get; private set; } = new IconCache();
        private IconCache() { }

        #endregion

        private Dictionary<string, ImageSource> icons = new Dictionary<string, ImageSource>();

        public ImageSource GetIcon(string iconPath)
        {
            if (!icons.ContainsKey(iconPath))
            {
                icons.Add(iconPath, new ImageSourceConverter().ConvertFromString($"pack://application:,,,/LevelEditorPlugin;component/{iconPath}") as ImageSource);
            }
            return icons[iconPath];
        }
    }

    public interface ITimelineEntityProviderTrack
    {
        Entities.Entity Entity { get; }
    }

    public class TimelineNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeline = value as Entities.TimelineEntity;
            if (timeline != null)
            {
                string strValue = timeline.Data.__Id;
                return strValue;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrackWrapper : INotifyPropertyChanged
    {
        public IEnumerable<TrackWrapper> Tracks => tracks;
        public Entities.TimelineTrack Track => track;
        public string TrackName => (track is Entities.ITimelineCustomTrackName) ? (track as Entities.ITimelineCustomTrackName).DisplayName : track.DisplayName;
        public string ToolTip => toolTip;
        public ImageSource Icon => icon;
        public bool IsEntityTrack => (track is ITimelineEntityProviderTrack);
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged();
                    ExpandedChanged?.Invoke(this, new RoutedEventArgs());
                }
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                    SelectionChanged.Invoke(this, new ComponentSelectionChangedEventArgs(track, IsSelected));
                }
            }
        }
        public ICommand SelectEntityInSceneCommand => new RelayCommand(OnSelectEntityInScene);

        private bool isSelected;
        private bool isExpanded;
        private Entities.TimelineTrack track;
        private List<TrackWrapper> tracks = new List<TrackWrapper>();
        private string toolTip;
        private ImageSource icon;

        public TrackWrapper(Entities.TimelineTrack inTrack, Entities.ReferenceObject layerEntity)
        {
            inTrack.Initialize(layerEntity);

            track = inTrack;
            isExpanded = false;
            toolTip = track.GetType().Name;
            
            if (!string.IsNullOrEmpty(track.Icon))
            {
                icon = IconCache.Instance.GetIcon(track.Icon);
            }

            foreach (var childTrack in track.Tracks)
            {
                tracks.Add(new TrackWrapper(childTrack, layerEntity));
            }
        }

        private void OnSelectEntityInScene(object data)
        {
            SelectEntityInScene?.Invoke(this, new SelectedEntityChangedEventArgs((track as ITimelineEntityProviderTrack).Entity, null));
        }

        public event EventHandler<SelectedEntityChangedEventArgs> SelectEntityInScene;
        public event EventHandler<ComponentSelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<RoutedEventArgs> ExpandedChanged;

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

    public class TrackDataWrapper : INotifyPropertyChanged
    {
        public IEnumerable<TrackDataWrapper> DataTracks => dataTracks;
        public object TrackData => track.Track;

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private TrackWrapper track;
        private List<TrackDataWrapper> dataTracks = new List<TrackDataWrapper>();
        private bool isSelected;
        private bool isExpanded;

        public TrackDataWrapper(TrackWrapper inTrack)
        {
            track = inTrack;
            track.ExpandedChanged += (o, e) => { IsExpanded = track.IsExpanded; };
            track.SelectionChanged += (o, e) => { IsSelected = track.IsSelected; };
            isExpanded = track.IsExpanded;

            foreach (var childTrack in track.Tracks)
            {
                dataTracks.Add(new TrackDataWrapper(childTrack));
            }
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

    public interface ITimelineUpdateProvider
    {
        void TimelinePlaybackUpdate(float currentTime);
    }

    public class TimelineViewModel : IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Timeline";
        public string UniqueId => "UID_LevelEditor_Timeline";
        public string Icon => "Images/Timeline.png";
        public DockManager DockManager => dockManager;
        public ICommand InitializeCommand => new RelayCommand(Initialize);
        public ICommand UnloadCommand => new RelayCommand(Unload);
        public ICommand HideCommand => new RelayCommand(Hide);
        public ICommand ShowCommand => new RelayCommand(Show);
        public ICommand DataModifiedCommand => new RelayCommand(PropertyGridDataModified);
        public IEnumerable<ToolbarItem> ToolbarItems => toolbarItems;
        public IEnumerable<TrackWrapper> Tracks
        {
            get => tracks;
            set
            {
                if (tracks != value)
                {
                    tracks = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public IEnumerable<TrackDataWrapper> DataTracks
        {
            get => dataTracks;
            set
            {
                if (dataTracks != value)
                {
                    dataTracks = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float PlaybackTime => playbackTime;
        public float StartTime => (selectedTimeline != null) ? selectedTimeline.Data.StartTime : 0.0f;
        public float EndTime => (selectedTimeline != null) ? selectedTimeline.Data.EndTime : 0.0f;

        private ToolbarAssetEditor owner;
        private DockManager dockManager;
        private List<ToolbarItem> toolbarItems;

        private Entities.TimelineEntity selectedTimeline;
        private IEnumerable<TrackWrapper> tracks;
        private IEnumerable<TrackDataWrapper> dataTracks;
        private Layers.SceneLayer selectedLayer;

        private bool isPlaying;
        private float playbackTime;
        private Timer playbackTimer;
        private Stopwatch elapsedTime;

        private RelayCommand startTimeUpdateCommand;
        private RelayCommand endTimeUpdateCommand;
        private RelayCommand timelinesUpdateCommand;

        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;

        public TimelineViewModel(ToolbarAssetEditor inOwner)
        {
            owner = inOwner;

            dockManager = new DockManager(inOwner);
            dockManager.LoadFromConfig("Timeline", new DockManager.DockManagerConfigData()
            {
                Layouts = new List<DockManager.DockLayoutData>()
                    {
                        new DockManager.DockLayoutData()
                        {
                            UniqueId = "UID_LevelEditor_Properties",
                            IsVisible = true,
                            IsSelected = true,
                            Location = DockLocation.TopRight
                        }
                    }
            });

            toolbarItems = RegisterToolbarItems();
        }

        public List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new ComboBoxToolbarItem("", "Current timeline to edit", null, null, new RelayCommand((o) => { ChangeTimeline(o as Entities.TimelineEntity); }), out timelinesUpdateCommand, 0.0, new TimelineNameConverter()),
                new DividerToolbarItem(),
                new TextBoxToolbarItem("Start:", "The starting time of the timeline", "0.0", out startTimeUpdateCommand),
                new TextBoxToolbarItem("End:", "The ending time of the timeline", "10.0", out endTimeUpdateCommand),
                new DividerToolbarItem(),
                new RegularToolbarItem("", "", "FrostyEditor/Images/Play.png", new RelayCommand((o) => { PlaybackTimeline(); }, (o) => { return !IsPlaying; })),
                new DividerToolbarItem(),
                new DockingToolbarItem("", "Show/Hide properties tab", "Images/Properties.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new PropertiesViewModel(this))), DockManager, "UID_LevelEditor_Properties")
            };
        }

        public void Initialize(object obj)
        {
            var layer = (owner as IEditorProvider).RootLayer;
            if (owner is SpatialEditor)
            {
                layer = (owner as SpatialEditor).SelectedLayer;
                if (layer == null)
                {
                    layer = (owner as IEditorProvider).RootLayer;
                }
            }

            playbackTimer = new Timer(OnUpdatePlayback, null, Timeout.Infinite, Timeout.Infinite);
            SetLayer(layer);

            if (owner is LevelEditor)
            {
                (owner as LevelEditor).SelectedLayerChanged += OnSelectedLayerChanged;
            }

            DockManager.AddItemOnLoad(new PropertiesViewModel(this));
            elapsedTime = new Stopwatch();
        }

        public void Unload(object obj)
        {
            if (owner is LevelEditor)
            {
                (owner as LevelEditor).SelectedLayerChanged -= OnSelectedLayerChanged;
            }

            DockManager.SaveToConfig("Timeline");
            DockManager.Shutdown();
        }

        public void Hide(object obj)
        {
            DockManager.HideFloatingWindows();
        }

        public void Show(object obj)
        {
            DockManager.ShowFloatingWindows();
        }

        private void RecursiveSetSelectionChanged(TrackWrapper wrapper)
        {
            wrapper.SelectionChanged += SelectedTrackChanged;
            wrapper.SelectEntityInScene += SelectEntityInScene;

            foreach (var child in wrapper.Tracks)
                RecursiveSetSelectionChanged(child);
        }

        private void OnSelectedLayerChanged(object sender, SelectedLayerChangedEventArgs e)
        {
            SetLayer(e.NewSelection);
        }

        private void SelectEntityInScene(object sender, SelectedEntityChangedEventArgs e)
        {
            (owner as IEditorProvider).SelectEntity(e.NewSelection);
        }

        private void SetLayer(Layers.SceneLayer layer)
        {
            selectedLayer = layer;

            List<Entities.Entity> entities = new List<Entities.Entity>();
            layer.CollectTimelines(entities);

            List<Entities.TimelineEntity> timelines = new List<Entities.TimelineEntity>();
            timelines.AddRange(entities.Select(e => e as Entities.TimelineEntity));

            timelinesUpdateCommand.Execute(timelines);
        }

        private void ChangeTimeline(Entities.TimelineEntity newTimeline)
        {
            if (selectedTimeline == newTimeline)
                return;

            if (newTimeline != null)
            {
                selectedTimeline = newTimeline;
                List<TrackWrapper> newTracks = new List<TrackWrapper>();
                List<TrackDataWrapper> newDataTracks = new List<TrackDataWrapper>();

                var refEntity = selectedLayer.Entity as Entities.ReferenceObject;
                foreach (var track in selectedTimeline.Tracks)
                {
                    var trackWrapper = new TrackWrapper(track, refEntity);
                    var dataWrapper = new TrackDataWrapper(trackWrapper);

                    RecursiveSetSelectionChanged(trackWrapper);

                    newTracks.Add(trackWrapper);
                    newDataTracks.Add(dataWrapper);
                }

                Tracks = newTracks;
                DataTracks = newDataTracks;

                startTimeUpdateCommand.Execute(newTimeline.Data.StartTime.ToString("F1"));
                endTimeUpdateCommand.Execute(newTimeline.Data.EndTime.ToString("F1"));

                NotifyPropertyChanged("StartTime");
                NotifyPropertyChanged("EndTime");
            }
            else
            {
                Tracks = null;
                DataTracks = null;

                startTimeUpdateCommand.Execute("0");
                endTimeUpdateCommand.Execute("0");

                NotifyPropertyChanged("StartTime");
                NotifyPropertyChanged("EndTime");
            }
        }

        private void SelectedTrackChanged(object sender, ComponentSelectionChangedEventArgs e)
        {
            Entities.TimelineTrack selectedTrack = e.Entity as Entities.TimelineTrack;
            if (selectedTrack == null)
            {
                SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(selectedTimeline, null));
                return;
            }

            SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(selectedTrack, null));
        }

        private void PlaybackTimeline()
        {
            IsPlaying = true;
            elapsedTime.Start();
            playbackTimer.Change(10, Timeout.Infinite);
        }

        private void OnUpdatePlayback(object state)
        {
            if (playbackTime > selectedTimeline.Data.EndTime)
            {
                elapsedTime.Stop();
                IsPlaying = false;

                playbackTime = 0;
                selectedTimeline.Update(playbackTime);
            }
            else
            {
                playbackTime += elapsedTime.ElapsedMilliseconds / 1000.0f;
                elapsedTime.Restart();

                playbackTimer.Change(Timeout.Infinite, Timeout.Infinite);
                selectedTimeline.Update(playbackTime);
                playbackTimer.Change(10, Timeout.Infinite);
            }

            NotifyPropertyChanged("PlaybackTime");

            if (owner is ITimelineUpdateProvider)
            {
                (owner as ITimelineUpdateProvider).TimelinePlaybackUpdate(playbackTime);
            }
        }

        private void PropertyGridDataModified(object obj)
        {
            //var args = obj as PropertyGridModifiedEventArgs;
            //if (args.IsUndoAction)
            //{
            //    LoadedAssetManager.Instance.UndoUpdate((layer.Entity as Entities.ReferenceObject).Blueprint);
            //}
            //else
            //{
            //    LoadedAssetManager.Instance.UpdateAsset((layer.Entity as Entities.ReferenceObject).Blueprint);
            //}

            //modifiedData = obj;
            //NotifyPropertyChanged("ModifiedData");
            //modifiedData = null;
            //NotifyPropertyChanged("ModifiedData");
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
