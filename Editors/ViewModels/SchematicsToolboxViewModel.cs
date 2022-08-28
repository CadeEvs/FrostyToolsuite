using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Attributes;
using FrostySdk.Managers;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Library.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace LevelEditorPlugin.Editors
{
    public interface IDragDropSourceProvider
    {
        bool InitiateDrag(out object dataToDrag, out FrameworkElement optionalVisual);
    }

    public interface IDragDropTargetProvider
    {
        bool IsValidDropSource(IDataObject draggedData);
        void ProcessDrop(IDataObject draggedData, Point dropPoint);
    }

    public static class DragDropExtension
    {
        public static readonly DependencyProperty IsDragDropSourceProperty = DependencyProperty.RegisterAttached("IsDragDropSource", typeof(bool), typeof(DragDropExtension), new PropertyMetadata(OnIsDragDropSourceChanged));
        public static readonly DependencyProperty IsDragDropTargetProperty = DependencyProperty.RegisterAttached("IsDragDropTarget", typeof(bool), typeof(DragDropExtension), new PropertyMetadata(OnIsDragDropTargetChanged));

        public static readonly DependencyProperty IsDragDropDataExplorerProperty = DependencyProperty.RegisterAttached("IsDragDropDataExplorer", typeof(bool), typeof(DragDropExtension), new PropertyMetadata(OnIsDragDropDataExplorerChanged));

        public static bool GetIsDragDropSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragDropSourceProperty);
        }
        public static void SetIsDragDropSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragDropSourceProperty, value);
        }

        public static bool GetIsDragDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragDropTargetProperty);
        }
        public static void SetIsDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragDropTargetProperty, value);
        }

        public static bool GetIsDragDropDataExplorer(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragDropDataExplorerProperty);
        }
        public static void SetIsDragDropDataExplorer(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragDropDataExplorerProperty, value);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private static void OnIsDragDropSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            Control ctrl = d as Control;
            if (ctrl != null)
            {
                IDragDropSourceProvider dragDropSourceProvider = ctrl.DataContext as IDragDropSourceProvider;
                if (dragDropSourceProvider != null)
                {
                    ctrl.MouseMove += (o, args) =>
                    {
                        if (args.LeftButton == MouseButtonState.Pressed)
                        {
                            object dataToDrag = null;
                            FrameworkElement visual = null;

                            if (dragDropSourceProvider.InitiateDrag(out dataToDrag, out visual))
                            {
                                Window win = null;
                                if (visual != null)
                                {
                                    win = new Window();
                                    win.Content = visual;
                                    win.WindowStyle = WindowStyle.None;
                                    win.Background = null;
                                    win.Opacity = 0.5;
                                    win.AllowsTransparency = true;
                                    win.ShowInTaskbar = false;
                                    win.Width = visual.Width;
                                    win.Height = visual.Height;
                                    win.Topmost = true;
                                    win.IsHitTestVisible = false;
                                    win.Focusable = false;
                                    win.IsEnabled = false;
                                    win.ShowActivated = false;
                                    win.SourceInitialized += (obj, a) =>
                                    {
                                        IntPtr hwnd = new WindowInteropHelper(obj as Window).Handle;
                                        SetWindowExTransparent(hwnd);
                                    };
                                    win.Show();
                                }

                                GiveFeedbackEventHandler handler = (obj, a) =>
                                {
                                    Point mousePos = GetMousePosition();
                                    win.Left = mousePos.X - (visual.Width * 0.5);
                                    win.Top = mousePos.Y - (visual.Height * 0.5);
                                };

                                DragDrop.AddGiveFeedbackHandler(ctrl, handler);
                                DragDrop.DoDragDrop(ctrl, dataToDrag, DragDropEffects.Move);
                                DragDrop.RemoveGiveFeedbackHandler(ctrl, handler);

                                win.Close();
                            }
                        }
                    };
                }
            }
        }

        private static void OnIsDragDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            Control ctrl = d as Control;
            if (ctrl != null)
            {
                IDragDropTargetProvider dragDropTargetProvider = ctrl.DataContext as IDragDropTargetProvider;
                if (dragDropTargetProvider == null)
                {
                    dragDropTargetProvider = ctrl.TemplatedParent as IDragDropTargetProvider;
                }

                if (dragDropTargetProvider != null)
                {
                    ctrl.AllowDrop = true;
                    DragEventHandler dropHandler = null;

                    dropHandler = (o, args) =>
                    {
                        dragDropTargetProvider.ProcessDrop(args.Data, ctrl.PointFromScreen(GetMousePosition()));
                        DragDrop.RemoveDropHandler(ctrl, dropHandler);
                    };

                    DragDrop.AddDragEnterHandler(ctrl, (o, args) =>
                    {
                        if (dragDropTargetProvider.IsValidDropSource(args.Data))
                        {
                            DragDrop.AddDropHandler(ctrl, dropHandler);
                            args.Effects = DragDropEffects.Move;
                            args.Handled = true;
                        }
                        else
                        {
                            args.Effects = DragDropEffects.None;
                            args.Handled = true;
                        }
                    });
                    DragDrop.AddDragLeaveHandler(ctrl, (o, args) =>
                    {
                        DragDrop.RemoveDropHandler(ctrl, dropHandler);
                        Mouse.OverrideCursor = null;
                    });

                }
            }
        }

        private static void OnIsDragDropDataExplorerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            FrostyDataExplorer dataExplorer = d as FrostyDataExplorer;
            if (dataExplorer != null)
            {
                ListView listView = ReflectionUtils.GetPrivateField(typeof(FrostyDataExplorer), App.EditorWindow.DataExplorer, "m_assetListView") as ListView;
                listView.MouseMove += (o, args) =>
                {
                    if (args.LeftButton == MouseButtonState.Pressed && listView.SelectedItem != null)
                    {
                        object dataToDrag = new DataExplorerDropData() { Entry = dataExplorer.SelectedAsset as EbxAssetEntry };
                        FrameworkElement visual = null;

                        {
                            Window win = null;
                            if (visual != null)
                            {
                                win = new Window();
                                win.Content = visual;
                                win.WindowStyle = WindowStyle.None;
                                win.Background = null;
                                win.Opacity = 0.5;
                                win.AllowsTransparency = true;
                                win.ShowInTaskbar = false;
                                win.Width = visual.Width;
                                win.Height = visual.Height;
                                win.Topmost = true;
                                win.IsHitTestVisible = false;
                                win.Focusable = false;
                                win.IsEnabled = false;
                                win.ShowActivated = false;
                                win.SourceInitialized += (obj, a) =>
                                {
                                    IntPtr hwnd = new WindowInteropHelper(obj as Window).Handle;
                                    SetWindowExTransparent(hwnd);
                                };
                                win.Show();
                            }

                            GiveFeedbackEventHandler handler = (obj, a) =>
                            {
                                if (visual == null)
                                    return;

                                Point mousePos = GetMousePosition();
                                win.Left = mousePos.X - (visual.Width * 0.5);
                                win.Top = mousePos.Y - (visual.Height * 0.5);
                            };

                            DragDrop.AddGiveFeedbackHandler(listView, handler);
                            DragDrop.DoDragDrop(listView, dataToDrag, DragDropEffects.Move);
                            DragDrop.RemoveGiveFeedbackHandler(listView, handler);

                            if (visual != null)
                            {
                                win.Close();
                            }
                        }
                    }
                };
            }
        }
    }

    public class SchematicsDropData
    {
        public Type DataType;
    }
    public class DataExplorerDropData
    {
        public EbxAssetEntry Entry;
    }

    public class ToolboxTypeItem : INotifyPropertyChanged
    {
        public string Name => m_name;
        public Type Type => m_type;
        public bool IsExpanded
        {
            get => m_isExpanded;
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string m_name;
        private bool m_isExpanded;
        private bool m_isSelected;
        private Type m_type;

        public ToolboxTypeItem(Type inType)
        {
            m_type = inType;
            m_name = m_type.Name;
            
            if (m_name.EndsWith("Entity"))
            {
                m_name = m_name.Remove(m_name.Length - 6);
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

    public class ToolboxFolderItem : INotifyPropertyChanged
    {
        public string Name => m_name;
        public IEnumerable<ToolboxTypeItem> Children => m_children;
        public bool IsExpanded
        {
            get => m_isExpanded;
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string m_name;
        private List<ToolboxTypeItem> m_children = new List<ToolboxTypeItem>();
        private bool m_isExpanded;
        private bool m_isSelected;

        public ToolboxFolderItem(string inName)
        {
            m_name = inName;
        }

        public void AddChild(Type childType)
        {
            m_children.Add(new ToolboxTypeItem(childType));
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

    public class SchematicsToolboxViewModel : Controls.IDockableItem, INotifyPropertyChanged, IDragDropSourceProvider
    {
        public string Header => "Toolbox";
        public virtual string UniqueId => "UID_LevelEditor_Toolbox";
        public string Icon => "Images/Toolbox.png";
        public IEnumerable<ToolboxFolderItem> Types
        {
            get => m_types;
            set
            {
                if (m_types != value)
                {
                    m_types = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string FilterText
        {
            get => m_filterText;
            set
            {
                m_filterText = value;
                FilterTextChanged();
            }
        }
        
        public ICommand SelectedTypeChangedCommand => new RelayCommand(SelectedTypeChanged);

        protected ToolboxTypeItem m_selectedType;
        
        private IEnumerable<ToolboxFolderItem> m_types;
        private IEnumerable<Type> m_allTypes;
        private string m_filterText;

        public SchematicsToolboxViewModel()
        {
            m_filterText = "";
            
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => IsTypeValid(t)))
            {
                types.Add(type);
            }
            m_allTypes = types;
            
            UpdateTypes();
        }

        public void UpdateTypes()
        {
            List<ToolboxFolderItem> modules = new List<ToolboxFolderItem>();
            foreach (Type type in m_allTypes)
            {
                EntityBindingAttribute attr = type.GetCustomAttribute<EntityBindingAttribute>();
                Type dataType = attr.DataType;
                EbxClassMetaAttribute metaAttr = dataType.GetCustomAttribute<EbxClassMetaAttribute>();

                if (metaAttr != null)
                {
                    if (m_filterText != "" && type.Name.IndexOf(m_filterText, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                    
                    ToolboxFolderItem folderItem = modules.Find(m => m.Name == metaAttr.Namespace);
                    if (folderItem == null)
                    {
                        folderItem = new ToolboxFolderItem(metaAttr.Namespace);
                        modules.Add(folderItem);
                    }

                    folderItem.AddChild(type);
                }
            }

            Types = modules;
        }
        
        public virtual bool InitiateDrag(out object dataToDrag, out FrameworkElement optionalVisual)
        {
            dataToDrag = null;
            optionalVisual = null;

            if (m_selectedType == null)
                return false;

            dataToDrag = new SchematicsDropData() { DataType = m_selectedType.Type };
            EntityBindingAttribute attr = m_selectedType.Type.GetCustomAttribute<EntityBindingAttribute>();

            Entity tmpEntity = (Entity)Activator.CreateInstance(m_selectedType.Type, new object[] { Activator.CreateInstance(attr.DataType), null });
            optionalVisual = new Controls.SchematicsPreviewControl(tmpEntity as ILogicEntity);

            return true;
        }

        protected virtual bool IsTypeValid(Type incomingType)
        {
            return incomingType.GetCustomAttribute<EntityBindingAttribute>() != null && incomingType.IsSubclassOf(typeof(LogicEntity));
        }

        private void SelectedTypeChanged(object newType)
        {
            if (newType is ToolboxFolderItem)
            {
                m_selectedType = null;
                return;
            }

            m_selectedType = newType as ToolboxTypeItem;
        }
        
        private void FilterTextChanged()
        {
            UpdateTypes();
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
