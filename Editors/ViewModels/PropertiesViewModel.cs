using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Attributes;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Library.Reflection;
using LevelEditorPlugin.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Frosty.Core.Managers;
using DisplayNameAttribute = FrostySdk.Attributes.DisplayNameAttribute;
using PointerRef = FrostySdk.Ebx.PointerRef;

namespace LevelEditorPlugin.Editors
{
    public class WrappedListMockData
    {
        public List<PointerRef> DataList { get; private set; } = new List<PointerRef>();
        public IList ActualList
        {
            get
            {
                IList tmpList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ListType));
                foreach (PointerRef pr in DataList)
                {
                    tmpList.Add(pr.GetObjectAs<object>());
                }
                return tmpList;
            }
        }
        public Type ListType { get; private set; }

        public WrappedListMockData(Type inListType)
        {
            ListType = inListType;
        }
    }

    public class PropertyGridRefreshArgs
    {
        public object Data { get; private set; }
        public string PathToCheck { get; private set; } = null;
        public bool SuppressCallbacks { get; private set; } = false;

        public PropertyGridRefreshArgs(object inData)
        {
            Data = inData;
            PathToCheck = null;
        }

        public PropertyGridRefreshArgs(object inData, string inPathToCheck, bool suppressCallbacks)
        {
            Data = inData;
            PathToCheck = inPathToCheck;
            SuppressCallbacks = suppressCallbacks;
        }
    }

    public class PropertyGridExtension : DependencyObject
    {
        public static readonly string PropertiesCategory = "Properties";
        public static readonly string MockDataCategory = "Mock Data";

        #region -- Update Property Grid Attached Property --

        public static readonly DependencyProperty UpdatePropertyGridProperty = DependencyProperty.RegisterAttached("UpdatePropertyGrid", typeof(object), typeof(PropertyGridExtension), new FrameworkPropertyMetadata(null, OnUpdatePropertyGridChanged));
        public static void SetUpdatePropertyGrid(UIElement element, object value)
        {
            element.SetValue(UpdatePropertyGridProperty, value);
        }
        public static object GetUpdatePropertyGrid(UIElement element)
        {
            return element.GetValue(UpdatePropertyGridProperty);
        }
        private static void OnUpdatePropertyGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            FrostyPropertyGrid propertyGrid = d as Frosty.Core.Controls.FrostyPropertyGrid;

            FieldInfo fieldInfo = propertyGrid.GetType().GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ObservableCollection<FrostyPropertyGridItemData> items = (ObservableCollection<Frosty.Core.Controls.FrostyPropertyGridItemData>)fieldInfo.GetValue(propertyGrid);
            PropertyGridRefreshArgs args = e.NewValue as PropertyGridRefreshArgs;

            foreach (FrostyPropertyGridItemData item in items)
            {
                if (item.DisplayName.Equals("Properties"))
                    continue;

                foreach (FrostyPropertyGridItemData subItem in item.Children)
                {
                    UpdateChildItems(subItem, args.PathToCheck, args.Data, args.SuppressCallbacks);
                }
            }
        }
        private static void UpdateChildItems(FrostyPropertyGridItemData item, string pathToCheck, object data, bool suppressCallbacks)
        {
            if (item.HasItems)
            {
                foreach (FrostyPropertyGridItemData subItem in item.Children)
                {
                    PropertyInfo pi = data.GetType().GetProperty(item.Name);
                    object subData = pi.GetValue(data);

                    IValueConverter valueConverter = item.GetType().GetField("_valueConverter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(item) as IValueConverter;
                    if (valueConverter != null)
                        subData = valueConverter.Convert(subData, null, null, System.Globalization.CultureInfo.CurrentCulture);

                    if (item.Path == pathToCheck && subData is IList)
                    {
                        UpdateArray(item);
                        break;
                    }

                    string subPath = (pathToCheck == item.Path) ? subItem.Path : pathToCheck;
                    UpdateChildItems(subItem, subPath, subData, suppressCallbacks);
                }
            }
            else
            {
                if (!item.IsReadOnly)
                {
                    if (pathToCheck == null || item.Path == pathToCheck)
                    {
                        PropertyInfo pi = data.GetType().GetProperty(item.Name);
                        object subData = item.Binding.GetValue();

                        if (pi != null)
                        {
                            subData = pi.GetValue(data);
                        }

                        IValueConverter valueConverter = item.GetType().GetField("_valueConverter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(item) as IValueConverter;
                        if (valueConverter != null)
                            subData = valueConverter.Convert(subData, null, null, System.Globalization.CultureInfo.CurrentCulture);

                        ForceValue(item, subData, suppressCallbacks);
                    }
                }
            }
        }

        /// <summary>
        /// Forces an array item to update its children and associated values
        /// </summary>
        /// <param name="item">The array item to update</param>
        private static void UpdateArray(FrostyPropertyGridItemData item)
        {
            Type itemType = item.GetType();

            if (item.HasItems)
            {
                if (item.Children != null)
                {
                    item.Children.Clear();
                    ReflectionUtils.CallPrivateMethod(itemType, item, "ProcessChildren");
                }
            }

            ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "Children");
            ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "HasItems");
            ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "Value");
        }

        /// <summary>
        /// A rewrite of the <see cref="FrostyPropertyGridItemData.ForceValue"/> function from <see cref="FrostyPropertyGridItemData"/> to allow
        /// suppressing of the premodified and standard modified callbacks.
        /// </summary>
        /// <param name="item">The <see cref="FrostyPropertyGridItemData"/> to force value on</param>
        /// <param name="value">The updated value</param>
        /// <param name="suppressCallbacks">Should premodified and modified be called or not</param>
        private static void ForceValue(FrostyPropertyGridItemData item, object value, bool suppressCallbacks = false)
        {
            Type itemType = item.GetType();

            if (!suppressCallbacks)
            {
                ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(item, item.Value, value);
                ReflectionUtils.CallPrivateMethod(itemType, item, "OnPreModified", item, args);

                if (args.Ignore)
                {
                    return;
                }
            }

            int index = -1;
            if (item.Binding is ArrayItemValueBinding)
            {
                index = ((ArrayItemValueBinding)item.Binding).index;
            }

            IValueBinding binding = item.Binding;
            if (binding != null)
            {
                binding.SetValue(value);
            }

            object oldValue = item.Value;
            ReflectionUtils.SetPrivateField(itemType, item, "_value", value);

            if (item.HasItems)
            {
                if (item.Children != null)
                {
                    item.Children.Clear();
                    ReflectionUtils.CallPrivateMethod(itemType, item, "ProcessChildren");
                    ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "Children");
                }
                ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "HasItems");
            }
            else if (item.Children != null)
            {
                item.Children.Clear();
                ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "Children");
                ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "HasItems");
            }

            item.IsModified = !value.Equals(ReflectionUtils.GetPrivateField(itemType, item, "_defaultValue"));
            ReflectionUtils.CallPrivateMethod(itemType, item, "NotifyPropertyChanged", "Value");

            if (!suppressCallbacks)
            {
                ReflectionUtils.CallPrivateMethod(itemType, item, "OnModified", item, new ItemModifiedEventArgs(item, value, oldValue, new ItemModifiedTypeArgs(ItemModifiedTypes.Assign, index)));
            }
        }

        #endregion

        #region -- Inject Property Grid Data Attached Property --

        // Allows the injection of new properties into the property grid that may not directly be related to the object
        // being edited.

        public static readonly DependencyProperty InjectPropertyGridDataProperty = DependencyProperty.RegisterAttached("InjectPropertyGridData", typeof(object), typeof(PropertyGridExtension), new FrameworkPropertyMetadata(null, OnInjectPropertyGridData));
        public static void SetInjectPropertyGridData(UIElement element, object value)
        {
            element.SetValue(InjectPropertyGridDataProperty, value);
        }
        public static object GetInjectPropertyGridData(UIElement element)
        {
            return element.GetValue(InjectPropertyGridDataProperty);
        }
        private static void OnInjectPropertyGridData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            IEnumerable<PropertyValue> overrideValues = e.NewValue as IEnumerable<PropertyValue>;
            if (overrideValues.Count() == 0)
                return;

            FrostyPropertyGrid propertyGrid = d as FrostyPropertyGrid;
            FieldInfo fieldInfo = propertyGrid.GetType().GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ObservableCollection<FrostyPropertyGridItemData> items = (ObservableCollection<FrostyPropertyGridItemData>)fieldInfo.GetValue(propertyGrid);
            FrostyPropertyGridItemData categoryItem = new FrostyPropertyGridItemData(PropertiesCategory) { IsCategory = true };

            categoryItem.Modified += (o, ev) => { propertyGrid.GetType().GetMethod("SubItem_Modified", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(propertyGrid, new[] { o, ev }); };
            categoryItem.PreModified += (o, ev) => { propertyGrid.GetType().GetMethod("SubItem_PreModified", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(propertyGrid, new[] { o, ev }); };

            foreach (PropertyValue overrideValue in overrideValues)
            {
                FrostyPropertyGridItemData pgid = new FrostyPropertyGridItemData(overrideValue.Name, overrideValue.Name, overrideValue.Value, "", categoryItem);
                categoryItem.Children.Add(pgid);
            }

            items.Add(categoryItem);
        }
        #endregion

        #region -- Inject Mock Data Attached Property --

        // Allows the injection of new properties into the property grid that may not directly be related to the object
        // being edited.

        public static readonly DependencyProperty InjectMockDataProperty = DependencyProperty.RegisterAttached("InjectMockData", typeof(object), typeof(PropertyGridExtension), new FrameworkPropertyMetadata(null, OnInjectMockData));
        public static void SetInjectMockData(UIElement element, object value)
        {
            element.SetValue(InjectMockDataProperty, value);
        }
        public static object GetInjectMockData(UIElement element)
        {
            return element.GetValue(InjectMockDataProperty);
        }
        private static void OnInjectMockData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            Type objectType = e.NewValue.GetType();
            FrostyPropertyGrid propertyGrid = d as FrostyPropertyGrid;
            List<FrostyPropertyGridItemData> childItems = new List<FrostyPropertyGridItemData>();
            FrostyPropertyGridItemData categoryItem = new FrostyPropertyGridItemData(MockDataCategory) { IsCategory = true };

            if (objectType == typeof(WrappedListMockData))
            {
                WrappedListMockData listData = e.NewValue as WrappedListMockData;
                FrostyPropertyGridItemData pgid = new FrostyPropertyGridItemData("Items", "Items", listData.DataList, "", categoryItem);

                pgid.Attributes.Add(new EbxFieldMetaAttribute(FrostySdk.IO.EbxFieldType.Array, listData.ListType.Name, FrostySdk.IO.EbxFieldType.Pointer));
                childItems.Add(pgid);
            }
            else
            {
                PropertyInfo[] pis = objectType.GetProperties();
                if (pis.Length == 0)
                    return;

                Array.Sort(pis, new PropertyComparer());

                foreach (PropertyInfo pi in pis)
                {
                    DisplayNameAttribute displayNameAttr = pi.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>();
                    string displayName = (displayNameAttr != null) ? displayNameAttr.Name : pi.Name;

                    FrostyPropertyGridItemData pgid = new FrostyPropertyGridItemData(displayName, pi.Name, pi.GetValue(e.NewValue), "", categoryItem);
                    pgid.Binding = new PropertyValueBinding(pi, e.NewValue);
                    pgid.Attributes.AddRange(pi.GetCustomAttributes<Attribute>());

                    childItems.Add(pgid);
                }
            }

            if (childItems.Count > 0)
            {
                FieldInfo fieldInfo = propertyGrid.GetType().GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);
                ObservableCollection<FrostyPropertyGridItemData> items = (ObservableCollection<FrostyPropertyGridItemData>)fieldInfo.GetValue(propertyGrid);

                categoryItem.Modified += (o, ev) => { propertyGrid.GetType().GetMethod("SubItem_Modified", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(propertyGrid, new[] { o, ev }); };
                categoryItem.PreModified += (o, ev) => { propertyGrid.GetType().GetMethod("SubItem_PreModified", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(propertyGrid, new[] { o, ev }); };

                foreach (FrostyPropertyGridItemData childItem in childItems)
                {
                    categoryItem.Children.Add(childItem);
                }

                items.Add(categoryItem);
            }
        }
        #endregion
    }

    public class PropertiesViewModel : Controls.IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Properties";
        public string UniqueId => "UID_LevelEditor_Properties";
        public string Icon => "Images/Properties.png";
        public object Data
        {
            get
            {
                if (m_selectedEntity == null)
                    return m_selectedObject;
                return m_selectedEntity.GetPropertyGridData();
            }
        }
        public object RefreshPropertyGrid { get; set; }
        public object ItemsToInject { get; set; }
        public object MockDataObject { get; set; }
        public RelayCommand OnPropertyGridModifiedCommand => new RelayCommand(PropertyGridModified);

        private IEditorProvider m_owner;
        private Entity m_selectedEntity;
        private object m_selectedObject;
        private ICommand m_dataModifiedCommand;

        public PropertiesViewModel(IEditorProvider inOwner, Entity currentSelection)
        {
            m_owner = inOwner;
            m_owner.SelectedEntityChanged += SelectedEntityChanged;
            m_owner.SelectedLayerChanged += SelectedLayerChanged;

            m_selectedEntity = currentSelection;
            if (m_selectedEntity != null)
            {
                m_selectedEntity.EntityModified += EntityModified;
            }
        }

        public PropertiesViewModel(SchematicsViewModel inOwner)
        {
            inOwner.SelectedEntityChanged += SelectedEntityChanged;
            inOwner.SelectedObjectChanged += SelectedObjectChanged;
            m_dataModifiedCommand = inOwner.DataModifiedCommand;
        }

        public PropertiesViewModel(TimelineViewModel inOwner)
        {
            inOwner.SelectedEntityChanged += SelectedEntityChanged;
        }

        public PropertiesViewModel(IEditorProvider inOwner, Entity currentSelection, ICommand dataModifiedCommand)
            : this(inOwner, currentSelection)
        {
            this.m_dataModifiedCommand = dataModifiedCommand;
        }

        private void SelectedObjectChanged(object sender, SelectedObjectChangedEventArgs e)
        {
            if (e.NewSelection != m_selectedEntity)
            {
                if (m_selectedEntity != null)
                {
                    m_selectedEntity.EntityModified -= EntityModified;
                    m_selectedEntity = null;
                }

                m_selectedObject = e.NewSelection;
                NotifyPropertyChanged("Data");
            }
        }

        private void SelectedLayerChanged(object sender, SelectedLayerChangedEventArgs e)
        {
            if (e.NewSelection.Entity != m_selectedEntity)
            {
                if (m_selectedEntity != null)
                {
                    m_selectedEntity.EntityModified -= EntityModified;
                }

                m_selectedEntity = e.NewSelection.Entity;

                if (m_selectedEntity != null)
                {
                    m_selectedEntity.EntityModified += EntityModified;
                }

                NotifyPropertyChanged("Data");
            }
        }

        private void SelectedEntityChanged(object sender, SelectedEntityChangedEventArgs e)
        {
            if (e.NewSelection != m_selectedEntity)
            {
                if (m_selectedEntity != null)
                {
                    m_selectedEntity.EntityModified -= EntityModified;
                }

                m_selectedEntity = e.NewSelection;
                NotifyPropertyChanged("Data");

                if (m_selectedEntity != null)
                {
                    m_selectedEntity.EntityModified += EntityModified;

                    ItemsToInject = m_selectedEntity.GetPropertyValues();
                    NotifyPropertyChanged("ItemsToInject");
                    ItemsToInject = null;
                    NotifyPropertyChanged("ItemsToInject");

                    MockDataObject = m_selectedEntity.GetMockDataObject();
                    NotifyPropertyChanged("MockDataObject");
                    MockDataObject = null;
                    NotifyPropertyChanged("MockDataObject");
                }
            }
        }

        private void EntityModified(object sender, EntityModifiedEventArgs e)
        {
            RefreshPropertyGrid = new PropertyGridRefreshArgs(m_selectedEntity.GetPropertyGridData(), e.OptionalParameterNameChanged, true);
            NotifyPropertyChanged("RefreshPropertyGrid");
            RefreshPropertyGrid = null;
            NotifyPropertyChanged("RefreshPropertyGrid");
        }

        private void PropertyGridModified(object args)
        {
            ItemModifiedEventArgs e = args as ItemModifiedEventArgs;

            ItemModifiedTypeArgs modifiedArgs = e.ModifiedArgs;
            IValueBinding binding = e.Item.Binding;
            string pathToCheck = e.Item.Path;
            object oldValue = e.OldValue;

            UndoManager.Instance.CommitUndo(new GenericUndoUnit("Property Grid Modification",
                (o) => { /* Do nothing for now */ },
                (o) =>
                {
                    switch (modifiedArgs.Type)
                    {
                        case ItemModifiedTypes.Add:
                        case ItemModifiedTypes.Insert:
                        {
                            // remove array element
                            IList list = binding.GetValue() as IList;
                            list.RemoveAt(e.ModifiedArgs.Index);
                            break;
                        }

                        case ItemModifiedTypes.Remove:
                        {
                            // add array item
                            IList list = binding.GetValue() as IList;
                            list.Insert(e.ModifiedArgs.Index, e.OldValue);
                            break;
                        }

                        case ItemModifiedTypes.Clear:
                        {
                            // add all items back
                            IList list = binding.GetValue() as IList;
                            IList oldList = e.OldValue as IList;

                            for (int i = 0; i < oldList.Count; i++)
                            {
                                list.Add(oldList[i]);
                            }
                            break;
                        }

                        case ItemModifiedTypes.Assign:
                        {
                            // standard assign
                            binding.SetValue(oldValue);
                            break;
                        }
                    }

                    if (this.m_selectedEntity != null)
                    {
                        RefreshPropertyGrid = new PropertyGridRefreshArgs(m_selectedEntity.GetPropertyGridData(), pathToCheck, true);
                        NotifyPropertyChanged("RefreshPropertyGrid");
                        RefreshPropertyGrid = null;
                        NotifyPropertyChanged("RefreshPropertyGrid");
                    }

                    if (m_dataModifiedCommand != null)
                    {
                        m_dataModifiedCommand.Execute(new PropertyGridModifiedEventArgs(e, true));
                    }
                }));

            if (e.Item.Parent.DisplayName == PropertyGridExtension.PropertiesCategory)
            {
                // find and update the property value
                PropertyValue propertyValue = m_selectedEntity.GetPropertyValues().Find(pv => pv.Name.Equals(e.Item.Name));
                propertyValue.SetValue(e.NewValue);

                // find the owner of the property values for this entity and update
                SubWorldReferenceObject parent = m_selectedEntity.FindAncestor<SubWorldReferenceObject>();
                LoadedAssetManager.Instance.UpdateAsset(parent.Blueprint);
            }
            else if (e.Item.Parent.DisplayName == PropertyGridExtension.MockDataCategory)
            {
                return;
            }

            if (m_selectedEntity != null)
            {
                m_selectedEntity.OnDataModified();
            }

            if (m_dataModifiedCommand != null)
            {
                m_dataModifiedCommand.Execute(new PropertyGridModifiedEventArgs(e, false));
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
}
