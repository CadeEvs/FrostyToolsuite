using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FrostySdk.IO;
using System.Collections;
using Frosty.Controls;
using FrostySdk.Attributes;
using System.ComponentModel;
using FrostySdk.Ebx;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Converters;
using FrostySdk;

namespace Frosty.Core.Controls
{
    #region -- Converters/Comparers --

    public class PropertyComparer : IComparer<PropertyInfo>
    {
        public int Compare(PropertyInfo x, PropertyInfo y)
        {
            FieldIndexAttribute attr1 = x.GetCustomAttribute<FieldIndexAttribute>();
            FieldIndexAttribute attr2 = y.GetCustomAttribute<FieldIndexAttribute>();

            int idx1 = attr1?.Index ?? -1;
            int idx2 = attr2?.Index ?? -1;

            //EbxFieldMetaAttribute attr1 = x.GetCustomAttribute<EbxFieldMetaAttribute>();
            //EbxFieldMetaAttribute attr2 = y.GetCustomAttribute<EbxFieldMetaAttribute>();
            //if (attr1 != null && attr2 != null)
            //    return attr1.Offset.CompareTo(attr2.Offset);
            //return x.MetadataToken.CompareTo(y.MetadataToken);
            return idx1.CompareTo(idx2);
        }
    }

    //public class FrostyItemToNameConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return "";

    //        string param = parameter as string;
    //        if (param == "Name")
    //        {
    //            Type itemType = value.GetType();
    //            FrostySdk.Attributes.DisplayNameAttribute attr = itemType.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>();

    //            Type overrideType = App.PluginManager.GetTypeOverride(itemType.Name);
    //            if (overrideType != null)
    //            {
    //                FrostySdk.Attributes.DisplayNameAttribute overrideAttr = overrideType.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>();
    //                if (overrideAttr != null)
    //                    attr = overrideAttr;
    //            }
                
    //            if (attr != null)
    //                return attr.Name;
    //            return value.GetType().Name;
    //        }
    //        else if (param == "Description")
    //        {
    //            Type valueType = value.GetType();
    //            FrostySdk.Attributes.DescriptionAttribute attr = valueType.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>();

    //            Type overrideType = App.PluginManager.GetTypeOverride(valueType.Name);
    //            if (overrideType != null)
    //            {
    //                FrostySdk.Attributes.DescriptionAttribute overrideAttr = overrideType.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>();
    //                if (overrideAttr != null)
    //                    attr = overrideAttr;
    //            }

    //            if (attr == null)
    //                return "";
    //            return attr.Description;
    //        }

    //        return "";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return null;
    //    }
    //}

    public class FrostyItemValuePaddingIncreaser : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Thickness t = (Thickness)values[0];
                bool b = (bool)values[1];
                return (b) ? t : new Thickness(t.Left + 14, 0, 0, 0);
            }
            catch (Exception)
            {
                return (Thickness)values[0];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FrostyArrayToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Collapsed;

            bool hasItems = (bool)value;
            return hasItems ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FrostyValueModifiedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FrostyObjectTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (values.Length > 1)
            {
                if (values[1] != null)
                {
                    FrostyPropertyGridItemData item = values[1] as FrostyPropertyGridItemData;
                    if (!item.IsCategory)
                    {
                        if (item.IsArrayChild)
                            item = item.Parent;

                        return param.Equals("Name") ? item.DisplayName : item.Description;
                    }
                }
            }

            if (values[0] == null)
                return "";

            Type type = values[0].GetType();
            if (param.Equals("Name"))
            {
                FrostySdk.Attributes.DisplayNameAttribute attr = type.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>();
                Type overrideType = App.PluginManager.GetTypeOverride(type.Name);
                if (overrideType != null)
                {
                    FrostySdk.Attributes.DisplayNameAttribute overrideAttr = overrideType.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>();
                    if (overrideAttr != null)
                        attr = overrideAttr;
                }

                return attr != null ? attr.Name : type.Name;
            }
            else
            {
                FrostySdk.Attributes.DescriptionAttribute attr = type.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>();
                Type overrideType = App.PluginManager.GetTypeOverride(type.Name);
                if (overrideType != null)
                {
                    FrostySdk.Attributes.DescriptionAttribute overrideAttr = overrideType.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>();
                    if (overrideAttr != null)
                        attr = overrideAttr;
                }

                return attr != null ? attr.Description : "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FrostyClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic obj = value;
            string param = (string)parameter;

            if (param.Equals("Name"))
                return (string)obj.__Id;

            AssetClassGuid guid = obj.GetInstanceGuid();
            return guid.ExportedGuid.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    #endregion

    public class AttributeList
    {
        private List<Attribute> attributes = new List<Attribute>();
        public void AddRange(IEnumerable<Attribute> inAttrs)
        {
            attributes.AddRange(inAttrs);
        }
        public void AddRangeAndReplace(IEnumerable<Attribute> inAttrs)
        {
            List<Attribute> tmpList = new List<Attribute>();
            foreach (var attr in inAttrs)
            {
                if (attr is EbxFieldMetaAttribute)
                    continue;
                if (attr is EbxClassMetaAttribute)
                    continue;

                for (int i = 0; i < attributes.Count; i++)
                {
                    if (attr.GetType() == attributes[i].GetType())
                    {
                        attributes.RemoveAt(i);
                        break;
                    }
                }
                tmpList.Add(attr);
            }
            attributes.AddRange(tmpList);
        }
        public T GetCustomAttribute<T>() where T : Attribute
        {
            foreach (var attr in attributes)
            {
                if (attr is T attribute)
                    return attribute;
            }
            return default(T);
        }

        public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute
        {
            foreach (var attr in attributes)
            {
                if (attr is T attribute)
                    yield return attribute;
            }
        }
    }

    // ...
    // Args
    // ...

    public class ItemModifiedTypeArgs
    {
        public ItemModifiedTypes Type { get; private set; }
        public int Index { get; private set; }

        public ItemModifiedTypeArgs(ItemModifiedTypes type, int index)
        {
            Type = type;
            Index = index;
        }
    }

    public class ItemModifiedEventArgs : EventArgs
    {
        public FrostyPropertyGridItemData Item { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
        public ItemModifiedTypeArgs ModifiedArgs { get; private set; }

        public ItemModifiedEventArgs(FrostyPropertyGridItemData item, object oldValue, object newValue, ItemModifiedTypeArgs modified)
        {
            Item = item;
            OldValue = oldValue;
            NewValue = newValue;
            ModifiedArgs = modified;
        }
    }

    public class ItemPreModifiedEventArgs : EventArgs
    {
        public FrostyPropertyGridItemData Item { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
        public bool Ignore { get; set; }

        public ItemPreModifiedEventArgs(FrostyPropertyGridItemData item, object oldValue, object newValue)
        {
            Item = item;
            OldValue = oldValue;
            NewValue = newValue;
            Ignore = false;
        }
    }

    // ...
    // Bindings
    // ...

    public interface IValueBinding
    {
        void SetValue(object value);

        object GetValue();
    }
    public class PropertyValueBinding : IValueBinding
    {
        private PropertyInfo property;
        private object instance;

        public PropertyValueBinding(PropertyInfo pi, object inst)
        {
            property = pi;
            instance = inst;
        }

        public void SetValue(object value)
        {
            property.SetValue(instance, value);
        }

        public object GetValue()
        {
            return property.GetValue(instance);
        }
    }
    public class ArrayItemValueBinding : IValueBinding
    {
        public IList list;
        public int index;

        public ArrayItemValueBinding(IList inList, int inIndex)
        {
            list = inList;
            index = inIndex;
        }

        public void SetValue(object value)
        {
            list[index] = value;
        }

        public object GetValue()
        {
            return list[index];
        }
    }

    // ...
    // Property Grid Item Data
    // ...

    [Flags]
    public enum FrostyPropertyGridItemFlags
    {
        None = 0,
        IsReference = 1
    }

    public enum ItemModifiedTypes
    {
        Assign,
        Add,
        Remove,
        Insert,
        Clear
    }

    public class FrostyPropertyGridItemData : INotifyPropertyChanged
    {
        public static readonly FrostyPropertyGridItemData[] EmptyList = new FrostyPropertyGridItemData[0];

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                NotifyPropertyChanged();
            }
        }
        public string Name { get => _internalName; private set => _internalName = value; }
        public string Description { get; set; }
        public object Value
        {
            get => _value;
            set
            {
                if (value != null && !value.Equals(_value))
                    ForceValue(value);
            }
        }
        public string Path
        {
            get
            {
                string retVal = _internalName;
                if (Parent != null)
                    retVal = Parent.Path + "." + retVal;
                return retVal.Trim('.');
            }
        }
        public bool IsCategory { get => _isCategory; set => _isCategory = value; }
        public bool IsExpanded { get => _isExpanded; set { _isExpanded = value; NotifyPropertyChanged(); } }
        public bool IsModified
        {
            get
            {
                if (!_isEnabled||_isReadOnly)
                    return false;
                if (Value != null)
                {
                    EbxFieldMetaAttribute meta = GetCustomAttribute<EbxFieldMetaAttribute>();
                    if (meta != null && meta.Type == EbxFieldType.Struct)
                        return Children.Any((FrostyPropertyGridItemData a) => a.IsModified);
                    
                    if (Value is IList list)
                        return list.Count > 0;
                }
                return _isModified;
            }
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsHidden { get => _isHidden; set { _isHidden = value; NotifyPropertyChanged(); } }
        public ObservableCollection<FrostyPropertyGridItemData> Children
        {
            get
            {
                if (_isCategory && _children == null)
                    _children = new ObservableCollection<FrostyPropertyGridItemData>();
                else if ((_children == null || _children.Count == 0))
                {
                    if(_children == null)
                        _children = new ObservableCollection<FrostyPropertyGridItemData>();
                    ProcessChildren();
                }
                return _children;
            }
        }
        public FrostyPropertyGridItemData Parent { get; set; }
        public string DependsOn { get; set; }
        public Type TypeEditor { get; set; }
        public IValueBinding Binding { get; set; }
        public bool IsArrayChild => Parent?.Value is IList;
        public bool IsPointerRef => Value is PointerRef;
        public List<EditorMetaDataAttribute> MetaData { get; private set; } = new List<EditorMetaDataAttribute>();
        public List<Attribute> Attributes { get; private set; } = new List<Attribute>();
        public FrostyPropertyGridItemFlags Flags => _flags;
        public bool HasItems
        {
            get
            {
                if (_isCategory)
                    return true;

                if (_value != null)
                {
                    if (_value is BoxedValueRef)
                        return true;

                    EbxClassMetaAttribute meta = _value.GetType().GetCustomAttribute<EbxClassMetaAttribute>();
                    object tmpValue = _value;
                    if (_valueConverter != null)
                        tmpValue = _valueConverter.Convert(tmpValue, typeof(object), null, null);

                    if (meta != null && meta.Type == EbxFieldType.Struct)
                    {
                        return tmpValue.GetType().GetProperties().Length > 0;
                    }
                    
                    if (tmpValue is IList list)
                    {
                        return GetCustomAttribute<HideChildrentAttribute>() == null && list.Count > 0;
                    }

                    if (tmpValue is PointerRef pr)
                    {
                        if ((_flags & FrostyPropertyGridItemFlags.IsReference) == 0)
                        {
                            if (pr.Type == PointerRefType.Internal)
                            {
                                return pr.Internal.GetType().GetProperties().Length > 0;
                            }
                        }
                    }
                }

                return false;
            }
        }

        private ObservableCollection<FrostyPropertyGridItemData> _children;
        private string _displayName;
        private string _internalName;
        private object _value;
        private object _defaultValue;
        private bool _isCategory;
        private bool _isModified;
        private bool _isEnabled;
        private bool _isReadOnly;
        private bool _isExpanded;
        private bool _isHidden;
        private FrostyPropertyGridItemFlags _flags;
        private IValueConverter _valueConverter;

        private BaseTypeOverride additionalData;

        public event EventHandler<ItemModifiedEventArgs> Modified;
        public event EventHandler<ItemPreModifiedEventArgs> PreModified;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Parent?.NotifyPropertyChanged(propertyName);
        }

        public FrostyPropertyGridItemData(string name, string internalName, object value, object defaultValue, FrostyPropertyGridItemData parent, FrostyPropertyGridItemFlags flags = FrostyPropertyGridItemFlags.None)
        {
            _internalName = internalName;
            _defaultValue = defaultValue;
            _isEnabled = true;
            _flags = flags;

            DisplayName = name;
            Value = value;
            Parent = parent;

            Type valueType = value.GetType();
            AttributeList attributes = new AttributeList();
            attributes.AddRange(valueType.GetCustomAttributes());

            Type overrideType = App.PluginManager.GetTypeOverride(valueType.Name);
            if (overrideType != null)
                attributes.AddRangeAndReplace(overrideType.GetCustomAttributes());

            if (attributes.GetCustomAttribute<ClassConverterAttribute>() != null)
            {
                Type classConverterType = attributes.GetCustomAttribute<ClassConverterAttribute>().Type;
                _valueConverter = (IValueConverter)Activator.CreateInstance(classConverterType);
            }
        }

        public FrostyPropertyGridItemData(string name)
        {
            _internalName = "";
            _isEnabled = true;

            DisplayName = name;
            IsCategory = true;
            IsExpanded = true;
        }

        public bool FilterPropertyName(string filterText, List<object> refObjects, bool doNotHideSubObjects = false)
        {
            if (_value is PointerRef pr)
            {
                if (pr.Type == PointerRefType.Internal)
                {
                    if (GetCustomAttribute<IsReferenceAttribute>() == null)
                    {
                        if (refObjects.Contains(pr.Internal))
                            return true;
                        refObjects.Add(pr.Internal);
                    }
                }
            }

            bool retVal = true;
            foreach (var item in Children)
            {
                string name = item.Name;
                if (item.IsArrayChild)
                    name = "";

                item.IsHidden = !doNotHideSubObjects && name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) == -1;
                if (!item.FilterPropertyName(filterText, refObjects, !item.IsHidden) || !item.IsHidden)
                    retVal = false;
            }
            if (!retVal)
            {
                IsHidden = retVal;
            }
            return retVal;
        }

        public bool FilterGuid(string guid, List<object> refObjects, bool doNotHideSubObjects = false)
        {
            if (_value is PointerRef pRef)
            {
                if (pRef.Type == PointerRefType.Internal)
                {
                    if (GetCustomAttribute<IsReferenceAttribute>() == null)
                    {
                        if (refObjects.Contains(pRef.Internal))
                            return true;
                        refObjects.Add(pRef.Internal);
                    }
                }
            }

            bool retVal = true;
            foreach (var item in Children)
            {
                item.IsHidden = !doNotHideSubObjects;
                if (item.Value is PointerRef pr)
                {
                    if (pr.Type == PointerRefType.Internal)
                    {
                        AssetClassGuid instGuid = ((dynamic)pr.Internal).GetInstanceGuid();
                        string instGuidString = instGuid.ToString();

                        if (instGuidString.Equals(guid))
                            item.IsHidden = false;
                    }
                    else if (pr.Type == PointerRefType.External)
                    {
                        string fileGuidString = pr.External.FileGuid.ToString();
                        string classGuidString = pr.External.ClassGuid.ToString();

                        if (classGuidString.Equals(guid) || fileGuidString.Equals(guid))
                            item.IsHidden = false;
                    }
                }
                if (item.Value is AssetClassGuid acg)
                {
                    string exportedGuidString = acg.ExportedGuid.ToString();

                    if (exportedGuidString.Equals(guid))
                        item.IsHidden = false;
                }

                if (!item.FilterGuid(guid, refObjects, !item.IsHidden) || !item.IsHidden)
                    retVal = false;
            }

            if (!retVal)
            {
                IsHidden = false;
            }
            return retVal;
        }

        public void AddChild(object value, object defValue)
        {
            IList list = (IList)_value;
            int index = list.Count;

            ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(this, null, value);
            OnPreModified(this, args);
            if (args.Ignore)
                return;

            list.Add(value);

            if (_children != null)
            {
                FrostyPropertyGridItemData newItem = new FrostyPropertyGridItemData("[" + index + "]", "[" + index + "]", value, defValue, this, Flags) {Binding = new ArrayItemValueBinding(list, index)};
                newItem.Attributes.AddRange(Attributes);

                _children.Add(newItem);
            }

            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("IsModified");
            NotifyPropertyChanged("HasItems");

            OnModified(this, new ItemModifiedEventArgs(this, null, value, new ItemModifiedTypeArgs(ItemModifiedTypes.Add, index)));
        }

        public void InsertChild(object value, object defValue, FrostyPropertyGridItemData insertLocation, int insertDir)
        {
            IList list = (IList)_value;

            ArrayItemValueBinding binding = (ArrayItemValueBinding)insertLocation.Binding;
            int index = binding.index + Math.Max(insertDir, 0);

            ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(this, null, value);
            OnPreModified(this, args);
            if (args.Ignore)
                return;

            if (index > list.Count)
                list.Add(value);
            else
                list.Insert(index, value);

            if (_children != null)
            {
                FrostyPropertyGridItemData newItem = new FrostyPropertyGridItemData("[" + index + "]", "[" + index + "]", value, defValue, this, Flags) {Binding = new ArrayItemValueBinding(list, index)};
                newItem.Attributes.AddRange(Attributes);

                if (index > _children.Count)
                    _children.Add(newItem);
                else
                {
                    _children.Insert(index, newItem);

                    // recalculate all value bindings for remaining children
                    for (int i = index; i < _children.Count; i++)
                    {
                        FrostyPropertyGridItemData child = _children[i];

                        child.Name = child.DisplayName = "[" + i + "]";
                        child.Binding = new ArrayItemValueBinding(list, i);
                    }
                }
            }

            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("IsModified");
            NotifyPropertyChanged("HasItems");

            OnModified(this, new ItemModifiedEventArgs(this, null, value, new ItemModifiedTypeArgs(ItemModifiedTypes.Insert, index)));
        }

        public void RemoveChild(FrostyPropertyGridItemData item)
        {
            IList list = (IList)Value;
            object oldItem = item.Value;

            ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(this, oldItem, null);
            OnPreModified(this, args);
            if (args.Ignore)
                return;

            list.Remove(item.Value);

            int index = -1;
            if (_children != null)
            {
                index = _children.IndexOf(item);
                _children.RemoveAt(index);

                // recalculate all value bindings for remaining children
                for (int i = index; i < _children.Count; i++)
                {
                    FrostyPropertyGridItemData child = _children[i];

                    child.Name = child.DisplayName = "[" + i + "]";
                    child.Binding = new ArrayItemValueBinding(list, i);
                }
            }

            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("IsModified");
            NotifyPropertyChanged("HasItems");

            OnModified(this, new ItemModifiedEventArgs(this, oldItem, null, new ItemModifiedTypeArgs(ItemModifiedTypes.Remove, index)));
        }

        public void ClearChildren()
        {
            IList list = (IList)Value;
            if (list.Count == 0)
                return;

            int oldCount = list.Count;
            ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(this, oldCount, 0);
            OnPreModified(this, args);
            if (args.Ignore)
                return;

            IList oldList = (IList)Activator.CreateInstance(list.GetType());
            for (int i = 0; i < list.Count; i++)
            {
                oldList.Add(list[i]);
            }

            list.Clear();
            _children?.Clear();

            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("IsModified");
            NotifyPropertyChanged("HasItems");

            OnModified(this, new ItemModifiedEventArgs(this, oldList, list, new ItemModifiedTypeArgs(ItemModifiedTypes.Clear, -1)));
        }

        public FrostyPropertyGridItemData FindChild(string name)
        {
            foreach (FrostyPropertyGridItemData child in _children)
            {
                if (child._internalName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return child;
            }
            return null;
        }

        public T GetMetaDataAttribute<T>() where T : EditorMetaDataAttribute
        {
            foreach (EditorMetaDataAttribute attr in MetaData)
            {
                if (attr is T attribute)
                    return attribute;
            }
            return null;
        }

        public void ForceValue(object value)
        {
            ItemPreModifiedEventArgs args = new ItemPreModifiedEventArgs(this, _value, value);
            OnPreModified(this, args);
            if (args.Ignore)
                return;

            int index = -1;
            if (Binding is ArrayItemValueBinding)
            {
                index = ((ArrayItemValueBinding)Binding).index;
            }
            Binding?.SetValue(value);

            object oldValue = _value;
            _value = value;

            if (HasItems)
            {
                if (_children != null)
                {
                    _children.Clear();
                    ProcessChildren();
                    NotifyPropertyChanged("Children");
                }
                NotifyPropertyChanged("HasItems");
            }
            else
            {
                if (_children != null)
                {
                    _children.Clear();
                    NotifyPropertyChanged("Children");
                    NotifyPropertyChanged("HasItems");
                }
            }

            IsModified = !_value.Equals(_defaultValue);
            NotifyPropertyChanged("Value");

            OnModified(this, new ItemModifiedEventArgs(this, oldValue, _value, new ItemModifiedTypeArgs(ItemModifiedTypes.Assign, index)));
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            foreach (Attribute attr in Attributes)
            {
                if (attr is T attribute)
                    return attribute;
            }
            return default;
        }

        private void ProcessChildren()
        {
            if (_value != null)
            {
                EbxClassMetaAttribute meta = _value.GetType().GetCustomAttribute<EbxClassMetaAttribute>();

                object tmpValue = _value;
                object tmpDefValue = _defaultValue;

                if (tmpValue is BoxedValueRef)
                {
                    if ((tmpValue as BoxedValueRef).Value != null)
                    {
                        tmpValue = _valueConverter.Convert(tmpValue, typeof(object), null, null);
                        tmpDefValue = null;// Activator.CreateInstance(tmpValue.GetType());
                        meta = new EbxClassMetaAttribute(EbxFieldType.Struct);
                    }
                }

                if (meta != null && meta.Type == EbxFieldType.Struct)
                {
                    if (_valueConverter != null && !(_value is BoxedValueRef))
                    {
                        tmpValue = _valueConverter.Convert(tmpValue, typeof(object), null, null);
                        tmpDefValue = Activator.CreateInstance(tmpValue.GetType());
                    }

                    foreach (FrostyPropertyGridItemData childItem in ProcessClass(tmpValue, tmpDefValue, this))
                    {
                        _children.Add(childItem);
                        if (_valueConverter != null)
                        {
                            childItem.PropertyChanged += (o, e) =>
                            {
                                if (e.PropertyName == "Value")
                                    Value = _valueConverter.ConvertBack(tmpValue, typeof(object), _value, null);
                            };
                        }
                    }

                    if (_value.GetType().GetCustomAttribute<IsExpandedByDefaultAttribute>() != null)
                        IsExpanded = true;
                }
                else if (tmpValue is IList list)
                {
                    if (GetCustomAttribute<HideChildrentAttribute>() != null)
                        return;

                    for (int i = 0; i < list.Count; i++)
                    {
                        object listValue = list[i];
                        object listDefValue = null;

                        if (listValue != null)
                            listDefValue = Activator.CreateInstance(listValue.GetType());

                        FrostyPropertyGridItemData listItem = new FrostyPropertyGridItemData("[" + i + "]", "[" + i + "]", listValue, listDefValue, this, _flags) {Binding = new ArrayItemValueBinding(list, i)};
                        listItem.Attributes.AddRange(Attributes);
                        _children.Add(listItem);
                    }
                }
                else if (tmpValue is PointerRef pr)
                {
                    if ((_flags & FrostyPropertyGridItemFlags.IsReference) == 0)
                    {
                        object defValue = (pr.Internal != null) ? Activator.CreateInstance(pr.Internal.GetType()) : null;

                        foreach (FrostyPropertyGridItemData childItem in ProcessClass(pr.Internal, defValue, this))
                            _children.Add(childItem);
                    }
                }
            }
        }

        private void OnModified(object sender, ItemModifiedEventArgs args)
        {
            if (Parent != null)
            {
                Parent.OnModified(sender, args);
                return;
            }
            Modified?.Invoke(sender, args);
        }

        private void OnPreModified(object sender, ItemPreModifiedEventArgs args)
        {
            if (Parent != null)
            {
                Parent.OnPreModified(sender, args);
                return;
            }
            PreModified?.Invoke(sender, args);
        }

        private FrostyPropertyGridItemData[] ProcessClass(object value, object defValue, FrostyPropertyGridItemData parent)
        {
            if (value == null)
                return FrostyPropertyGridItemData.EmptyList;

            PropertyInfo[] pis = value.GetType().GetProperties();

            Type overrideType = App.PluginManager.GetTypeOverride(value.GetType().Name);
            object typeOverrideDefaultValue = null;

            if (overrideType != null)
            {
                additionalData = (BaseTypeOverride)Activator.CreateInstance(overrideType);
                additionalData.Original = value;
                additionalData.Load();

                typeOverrideDefaultValue = Activator.CreateInstance(overrideType);
                PropertyInfo[] newProperties = overrideType.GetProperties();

                pis = pis.Union(from p in newProperties
                                where pis.FirstOrDefault((PropertyInfo op) => op.Name == p.Name) == null
                                select p).ToArray<PropertyInfo>();
            }
            else
            {
                additionalData = null;
            }

            Array.Sort(pis, new PropertyComparer());

            List<FrostyPropertyGridItemData> items = new List<FrostyPropertyGridItemData>();
            foreach (PropertyInfo pi in pis)
            {
                AttributeList attributes = new AttributeList();
                attributes.AddRange(pi.GetCustomAttributes());

                if (overrideType != null)
                {
                    var overridePi = overrideType.GetProperty(pi.Name);
                    if (overridePi != null)
                    {
                        attributes.AddRangeAndReplace(overridePi.GetCustomAttributes());
                    }
                }

                if (attributes.GetCustomAttribute<IsHiddenAttribute>() != null)
                    continue;

                string name = pi.Name;
                if (attributes.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>() != null)
                    name = attributes.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>().Name;

                FrostyPropertyGridItemFlags flags = FrostyPropertyGridItemFlags.None;
                if (attributes.GetCustomAttribute<IsReferenceAttribute>() != null)
                    flags |= FrostyPropertyGridItemFlags.IsReference;
                
                object actualObject = value;
                object actualDefaultValue = defValue;
                if (overrideType != null && overrideType.GetProperties().Contains(pi))
                {
                    actualObject = additionalData;
                    actualDefaultValue = typeOverrideDefaultValue;
                }
                if (actualDefaultValue != null)
                    actualDefaultValue = pi.GetValue(actualDefaultValue);

                FrostyPropertyGridItemData subItem = new FrostyPropertyGridItemData(name, pi.Name, pi.GetValue(actualObject), actualDefaultValue, parent, flags) { Binding = new PropertyValueBinding(pi, actualObject) };

                if (attributes.GetCustomAttribute<FrostySdk.Attributes.IsReadOnlyAttribute>() != null)
                    subItem.IsReadOnly = true;
                if (attributes.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>() != null)
                    subItem.Description = attributes.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>().Description;
                if (attributes.GetCustomAttribute<DependsOnAttribute>() != null)
                    subItem.DependsOn = attributes.GetCustomAttribute<DependsOnAttribute>().Name;
                if (attributes.GetCustomAttribute<FrostySdk.Attributes.EditorAttribute>() != null)
                    subItem.TypeEditor = attributes.GetCustomAttribute<FrostySdk.Attributes.EditorAttribute>().EditorType;
                if (attributes.GetCustomAttribute<IsExpandedByDefaultAttribute>() != null)
                    subItem.IsExpanded = true;
                if (attributes.GetCustomAttribute<FixedSizeArrayAttribute>() != null)
                    subItem.IsEnabled = false;

                subItem.MetaData.AddRange(attributes.GetCustomAttributes<EditorMetaDataAttribute>());
                subItem.Attributes.AddRange(attributes.GetCustomAttributes<Attribute>());

                items.Add(subItem);
            }

            return items.ToArray();
        }
    }

    // ...
    // Property Grid Item
    // ...

    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FrostyPropertyGridItem))]
    [TemplatePart(Name = "ItemsHost", Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = "PART_Value", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_ArrayRemoveButton", Type = typeof(Button))]
    public class FrostyPropertyGridItem : TreeViewItem
    {
        private static readonly IReadOnlyDictionary<string, Type> StaticEditors = new Dictionary<string, Type>
        {
            //{ "String", typeof(FrostyStringEditor) },
            { "CString", typeof(FrostyCStringEditor) },
            { "String", typeof(FrostyStringEditor) },
            { "Boolean", typeof(FrostyBooleanEditor) },
            { "List`1", typeof(FrostyArrayEditor) },
            { "Byte", typeof(FrostyNumberEditor) },
            { "SByte", typeof(FrostyNumberEditor) },
            { "Int16", typeof(FrostyNumberEditor) },
            { "UInt16", typeof(FrostyNumberEditor) },
            { "Int32", typeof(FrostyNumberEditor) },
            { "UInt32", typeof(FrostyNumberEditor) },
            { "Int64", typeof(FrostyNumberEditor) },
            { "UInt64", typeof(FrostyNumberEditor) },
            { "Single", typeof(FrostyNumberEditor) },
            { "Double", typeof(FrostyNumberEditor) },
            { "PointerRef", typeof(FrostyPointerRefEditor) },
            { "ResourceRef", typeof(FrostyResourceRefEditor) },
            { "Guid", typeof(FrostyGuidEditor) }
        };

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;

            Button btn = GetTemplateChild("PART_ArrayRemoveButton") as Button;
            btn.Click += ArrayRemoveButton_Click;

            if (!item.IsCategory)
            {
                ContentControl value = GetTemplateChild("PART_Value") as ContentControl;
                UIElement elem = null;

                if (item.Value != null)
                {
                    string valueTypeName = item.Value.GetType().Name;
                    Type editorType = null;

                    if (item.TypeEditor != null)
                        editorType = item.TypeEditor;

                    if (editorType == null)
                    {
                        if (item.Name.Equals("__InstanceGuid"))
                        {
                            editorType = typeof(FrostyStructEditor);
                            item.IsReadOnly = true;
                        }
                        else if (StaticEditors.ContainsKey(valueTypeName))
                        {
                            editorType = StaticEditors[valueTypeName];
                        }
                        else if (item.Value is Enum)
                        {
                            editorType = typeof(FrostyEnumEditor);
                        }
                        else if (item.Value is IList)
                        {
                            editorType = typeof(FrostyArrayEditor);
                        }
                        else
                        {
                            editorType = App.PluginManager.GetTypeEditor(valueTypeName);
                            if (editorType == null)
                            {
                                editorType = typeof(FrostyStructEditor);
                            }
                        }
                    }

                    object editor = Activator.CreateInstance(editorType);
                    elem = (UIElement)editorType.GetMethod("CreateEditor").Invoke(editor, new object[] { item });
                }

                value.Content = elem;
            }

            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem
            {
                Header = "Copy",
                Icon = new Image
                {
                    Source = StringToBitmapSourceConverter.CopySource
                }
            };
            mi.Click += CopyMenuItem_Click;
            cm.Items.Add(mi);

            mi = new MenuItem
            {
                Header = "Paste",
                Icon = new Image
                {
                    Source = StringToBitmapSourceConverter.PasteSource
                }
            };
            mi.Click += PasteMenuItem_Click;
            BindingOperations.SetBinding(mi, IsEnabledProperty, new Binding("HasData") { Source = FrostyClipboard.Current });
            cm.Items.Add(mi);

            if (item.IsPointerRef)
            {
                cm.Items.Add(new Separator());

                mi = new MenuItem
                {
                    Header = "Copy Guid",
                    Icon = new Image
                    {
                        Source = StringToBitmapSourceConverter.CopySource
                    }
                };
                mi.Click += CopyGuidMenuItem_Click;
                cm.Items.Add(mi);
            }

            if (item.IsArrayChild)
            {
                cm.Items.Add(new Separator());

                mi = new MenuItem {Header = "Insert Before"};
                mi.Click += ArrayInsertBeforeMenuItem_Click;
                cm.Items.Add(mi);

                mi = new MenuItem {Header = "Insert After"};
                mi.Click += ArrayInsertAfterMenuItem_Click;
                cm.Items.Add(mi);
            }

            ContextMenu = cm;
        }

        /// <summary>
        /// Copies the PointerRef's guid to the clipboard
        /// </summary>
        private void CopyGuidMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;

            string guidToCopy = "";

            PointerRef pointerRef = (PointerRef)item.Value;
            if (pointerRef.Type == PointerRefType.Null)
                guidToCopy = "";
            else if (pointerRef.Type == PointerRefType.External)
                guidToCopy = pointerRef.External.ClassGuid.ToString();
            else
            {
                dynamic obj = pointerRef.Internal;
                guidToCopy = obj.GetInstanceGuid().ToString();
            }

            Clipboard.SetText(guidToCopy);
        }

        /// <summary>
        /// Copies the items data to the clipboard
        /// </summary>
        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;
            FrostyClipboard.Current.SetData(item.Value);
        }

        /// <summary>
        /// Tries to paste the currently copied clipboard data
        /// </summary>
        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FrostyClipboard.Current.HasData)
            {
                FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;
                if (item.IsEnabled && !item.IsReadOnly)
                {
                    if (FrostyClipboard.Current.IsType(item.Value.GetType()))
                    {
                        FrostyPropertyGrid pg = GetPropertyGrid();

                        if (pg.Asset != null)
                        {
                            // property grid is displaying an asset
                            item.Value = FrostyClipboard.Current.GetData(pg.Asset, App.AssetManager.GetEbxEntry(pg.Asset.FileGuid));
                        }
                        else
                        {
                            // property grid is displaying a custom EBX class
                            item.Value = FrostyClipboard.Current.GetData();
                        }
                    }
                }
            }
        }

        private FrostyPropertyGrid GetPropertyGrid()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (!(parent.GetType().IsSubclassOf(typeof(FrostyPropertyGrid)) || parent is FrostyPropertyGrid))
                parent = VisualTreeHelper.GetParent(parent);
            return (parent as FrostyPropertyGrid);
        }

        /// <summary>
        /// Inserts a new array element before the selected item
        /// </summary>
        private void ArrayInsertBeforeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;

            IList list = item.Parent.Value as IList;
            Type listType = list.GetType().GetGenericArguments()[0];

            item.Parent.InsertChild(Activator.CreateInstance(listType), Activator.CreateInstance(listType), item, -1);
        }

        /// <summary>
        /// Inserts a new array element after the selected item
        /// </summary>
        private void ArrayInsertAfterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;

            IList list = item.Parent.Value as IList;
            Type listType = list.GetType().GetGenericArguments()[0];

            item.Parent.InsertChild(Activator.CreateInstance(listType), Activator.CreateInstance(listType), item, 1);
        }

        /// <summary>
        /// Removes the selected array element
        /// </summary>
        private void ArrayRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            FrostyPropertyGridItemData item = (FrostyPropertyGridItemData)DataContext;
            item.Parent.RemoveChild(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FrostyPropertyGridItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrostyPropertyGridItem;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Subtract || e.Key == Key.Add)
            {
                // to stop +/- from expanding/collapsing
                e.Handled = true;

                if (e.OriginalSource is TextBox tb)
                {
                    // to ensure that textboxes still respond to +/-
                    int lastLocation = tb.SelectionStart;
                    tb.Text = tb.Text.Insert(lastLocation, (e.Key == Key.Subtract) ? "-" : "+");
                    tb.SelectionStart = lastLocation + 1;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            // to make sure right click selects item
            base.OnPreviewMouseRightButtonDown(e);
            IsSelected = true;
        }
    }

    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FrostyPropertyGridItem))]
    public class FrostyPropertyGridTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FrostyPropertyGridItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrostyPropertyGridItem;
        }
    }

    // ...
    // Property Grid
    // ...

    [TemplatePart(Name = "tv", Type = typeof(TreeView))]
    [TemplatePart(Name = PART_ClassesComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_ClassesRow, Type = typeof(RowDefinition))]
    [TemplatePart(Name = PART_FilterTextBox, Type = typeof(FrostyWatermarkTextBox))]
    [TemplatePart(Name = PART_FilterInProgresBorder, Type = typeof(Border))]
    [TemplatePart(Name = PART_FilterProgressBar, Type = typeof(ProgressBar))]
    public class FrostyPropertyGrid : Control
    {
        private const string PART_ClassesComboBox = "PART_ClassesComboBox";
        private const string PART_ClassesRow = "PART_ClassesRow";
        private const string PART_FilterTextBox = "PART_FilterTextBox";
        private const string PART_FilterInProgresBorder = "PART_FilterInProgresBorder";
        private const string PART_FilterProgressBar = "PART_FilterProgressBar";

        #region -- Properties --

        #region -- Object --
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(FrostyPropertyGrid), new FrameworkPropertyMetadata(null, OnObjectChanged));
        public object Object
        {
            get => GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }
        private static void OnObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrostyPropertyGrid pg = sender as FrostyPropertyGrid;
            if (args.NewValue != null)
            {
                pg.SetItems(pg.ProcessClassWithCategories(args.NewValue));
            }
        }
        #endregion

        #region -- Asset --
        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register("Asset", typeof(EbxAsset), typeof(FrostyPropertyGrid), new FrameworkPropertyMetadata(null, OnAssetChanged));
        public EbxAsset Asset
        {
            get => (EbxAsset)GetValue(AssetProperty);
            set => SetValue(AssetProperty, value);
        }
        private static void OnAssetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrostyPropertyGrid pg = sender as FrostyPropertyGrid;
            EbxAsset asset = args.NewValue as EbxAsset;
            pg.asset = asset;
        }
        #endregion

        #region -- Classes --
        public static readonly DependencyProperty ClassesProperty = DependencyProperty.Register("Classes", typeof(IEnumerable), typeof(FrostyPropertyGrid), new FrameworkPropertyMetadata(null, OnClassesChanged));
        public IEnumerable Classes
        {
            get => (IEnumerable)GetValue(ClassesProperty);
            set => SetValue(ClassesProperty, value);
        }
        private static void OnClassesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrostyPropertyGrid pg = sender as FrostyPropertyGrid;
            IEnumerable classes = args.NewValue as IEnumerable;
            pg.classes = classes;
            pg.SetClass(classes.Cast<object>().First());
        }
        #endregion

        #region -- HeaderVisible --
        public static readonly DependencyProperty HeaderVisibleProperty = DependencyProperty.Register("HeaderVisible", typeof(bool), typeof(FrostyPropertyGrid), new FrameworkPropertyMetadata(false));
        public bool HeaderVisible
        {
            get => (bool)GetValue(HeaderVisibleProperty);
            set => SetValue(HeaderVisibleProperty, value);
        }
        #endregion

        #region -- ClassViewVisible --
        public static readonly DependencyProperty ClassViewVisibleProperty = DependencyProperty.Register("ClassViewVisible", typeof(bool), typeof(FrostyPropertyGrid), new UIPropertyMetadata(true));
        public bool ClassViewVisible
        {
            get => (bool)GetValue(ClassViewVisibleProperty);
            set => SetValue(ClassViewVisibleProperty, value);
        }
        #endregion

        #region -- Modified --
        public static readonly DependencyProperty ModifiedProperty = DependencyProperty.Register("Modified", typeof(bool), typeof(FrostyPropertyGrid), new UIPropertyMetadata(false));
        public bool Modified
        {
            get => (bool)GetValue(ModifiedProperty);
            set => SetValue(ModifiedProperty, value);
        }
        #endregion

        #region -- InitialWidth --
        public static readonly DependencyProperty InitialWidthProperty = DependencyProperty.Register("InitialWidth", typeof(GridLength), typeof(FrostyPropertyGrid), new UIPropertyMetadata(new GridLength(1.0, GridUnitType.Star)));
        public GridLength InitialWidth
        {
            get => (GridLength)GetValue(InitialWidthProperty);
            set => SetValue(InitialWidthProperty, value);
        }
        #endregion

        #region -- FilterText --
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(FrostyPropertyGrid), new UIPropertyMetadata(""));
        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }
        #endregion

        #endregion

        public object SelectedClass => Object;

        public event EventHandler<ItemModifiedEventArgs> OnModified;
        public event EventHandler<ItemPreModifiedEventArgs> OnPreModified;

        private BaseTypeOverride additionalData;

        private TreeView tv;
        private FrostyWatermarkTextBox filterBox;
        private Border filterInProgressBorder;
        private ProgressBar filterProgressBar;
        private ObservableCollection<FrostyPropertyGridItemData> items;
        private FrostyPropertyGridItemData rootChild;
        private EbxAsset asset;
        private IEnumerable classes;

        public static readonly DependencyProperty OnPreModifiedCommandProperty = DependencyProperty.Register("OnPreModifiedCommand", typeof(ICommand), typeof(FrostyPropertyGrid), new UIPropertyMetadata(null));
        public ICommand OnPreModifiedCommand
        {
            get
            {
                return (ICommand)GetValue(OnPreModifiedCommandProperty);
            }
            set
            {
                SetValue(OnPreModifiedCommandProperty, value);
            }
        }

        public static readonly DependencyProperty OnModifiedCommandProperty = DependencyProperty.Register("OnModifiedCommand", typeof(ICommand), typeof(FrostyPropertyGrid), new UIPropertyMetadata(null));
        public ICommand OnModifiedCommand
        {
            get
            {
                return (ICommand)GetValue(OnModifiedCommandProperty);
            }
            set
            {
                SetValue(OnModifiedCommandProperty, value);
            }
        }

        static FrostyPropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyPropertyGrid), new FrameworkPropertyMetadata(typeof(FrostyPropertyGrid)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tv = GetTemplateChild("tv") as TreeView;
            filterBox = GetTemplateChild(PART_FilterTextBox) as FrostyWatermarkTextBox;
            filterInProgressBorder = GetTemplateChild(PART_FilterInProgresBorder) as Border;
            filterProgressBar = GetTemplateChild(PART_FilterProgressBar) as ProgressBar;

            tv.ItemsSource = items;
            filterBox.KeyUp += FilterBox_KeyUp;
            filterBox.LostFocus += FilterBox_LostFocus;
            //filterBox.TextChanged += FilterBox_TextChanged;

            if (asset != null)
                SetClass(asset.RootObject);
            else if (classes != null)
                SetClass(classes.Cast<object>().First());
        }

        private async void FilterBox_LostFocus(object sender, RoutedEventArgs e)
        {            
            string filterText = filterBox.Text;
            if (filterText == FilterText)
                return;

            filterBox.IsEnabled = false;
            tv.IsEnabled = false;
            filterProgressBar.Visibility = Visibility.Visible;
            filterInProgressBorder.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                List<object> refObjects = new List<object>();

                if (filterText.StartsWith("guid:"))
                {
                    string[] arr = filterText.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string guidValue = (arr.Length > 1) ? arr[1] : "0";

                    foreach (var item in items)
                    {
                        if (item.FilterGuid(guidValue.ToLower(), refObjects))
                            item.IsHidden = true;
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        if (item.FilterPropertyName(filterText, refObjects))
                            item.IsHidden = true;
                    }
                }
            });

            FilterText = filterText;
            filterBox.IsEnabled = true;
            tv.IsEnabled = true;
            filterProgressBar.Visibility = Visibility.Collapsed;
            filterInProgressBorder.Visibility = Visibility.Collapsed;

            GC.Collect();
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                filterBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        public void SetClass(object obj)
        {
            Object = obj;
        }

        protected void SetItems(FrostyPropertyGridItemData[] inItems)
        {
            if (items == null)
                items = new ObservableCollection<FrostyPropertyGridItemData>();
            items.Clear();
            foreach (FrostyPropertyGridItemData ti in inItems)
                items.Add(ti);

            if (tv != null)
                tv.ItemsSource = items;
        }

        protected virtual FrostyPropertyGridItemData[] ProcessClassWithCategories(object value)
        {
            if (value == null)
                return FrostyPropertyGridItemData.EmptyList;

            object defValue = Activator.CreateInstance(value.GetType());
            SortedDictionary<string, FrostyPropertyGridItemData> categories = new SortedDictionary<string, FrostyPropertyGridItemData>();

            rootChild = new FrostyPropertyGridItemData("");
            rootChild.Modified += SubItem_Modified;
            rootChild.PreModified += SubItem_PreModified;

            PropertyInfo[] pis = value.GetType().GetProperties();

            Type overrideType = App.PluginManager.GetTypeOverride(value.GetType().Name);
            object typeOverrideDefaultValue = null;

            if (overrideType != null)
            {
                additionalData = (BaseTypeOverride)Activator.CreateInstance(overrideType);
                additionalData.Original = value;
                additionalData.Load();

                typeOverrideDefaultValue = Activator.CreateInstance(overrideType);
                PropertyInfo[] newProperties = overrideType.GetProperties();

                pis = pis.Union(from p in newProperties
                                where pis.FirstOrDefault((PropertyInfo op) => op.Name == p.Name) == null
                                select p).ToArray<PropertyInfo>();
            }
            else
            {
                additionalData = null;
            }

            Array.Sort(pis, new PropertyComparer());

            foreach (PropertyInfo pi in pis)
            {
                AttributeList attributes = new AttributeList();
                attributes.AddRange(pi.GetCustomAttributes());

                if (overrideType != null)
                {
                    var overridePi = overrideType.GetProperty(pi.Name);
                    if (overridePi != null)
                    {
                        attributes.AddRangeAndReplace(overridePi.GetCustomAttributes());
                    }
                }

                if (attributes.GetCustomAttribute<IsHiddenAttribute>() != null)
                    continue;

                string category = "Misc";
                if (attributes.GetCustomAttribute<FrostySdk.Attributes.CategoryAttribute>() != null)
                    category = attributes.GetCustomAttribute<FrostySdk.Attributes.CategoryAttribute>().Name;

                if (!categories.ContainsKey(category))
                    categories.Add(category, new FrostyPropertyGridItemData(category));

                string name = pi.Name;
                if (attributes.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>() != null)
                    name = attributes.GetCustomAttribute<FrostySdk.Attributes.DisplayNameAttribute>().Name;

                FrostyPropertyGridItemFlags flags = FrostyPropertyGridItemFlags.None;
                if (attributes.GetCustomAttribute<IsReferenceAttribute>() != null)
                    flags |= FrostyPropertyGridItemFlags.IsReference;

                object actualObject = value;
                object actualDefaultValue = defValue;
                if (overrideType != null && overrideType.GetProperties().Contains(pi))
                {
                    actualObject = additionalData;
                    actualDefaultValue = typeOverrideDefaultValue;
                }

                if (pi.GetValue(actualObject) != null) // Checks if property object is null.
                {
                    FrostyPropertyGridItemData subItem = new FrostyPropertyGridItemData(name, pi.Name, pi.GetValue(actualObject), pi.GetValue(actualDefaultValue), rootChild, flags) { Binding = new PropertyValueBinding(pi, actualObject) };

                    if (attributes.GetCustomAttribute<FrostySdk.Attributes.IsReadOnlyAttribute>() != null)
                        subItem.IsReadOnly = true;
                    if (attributes.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>() != null)
                        subItem.Description = attributes.GetCustomAttribute<FrostySdk.Attributes.DescriptionAttribute>().Description;
                    if (attributes.GetCustomAttribute<DependsOnAttribute>() != null)
                        subItem.DependsOn = attributes.GetCustomAttribute<DependsOnAttribute>().Name;
                    if (attributes.GetCustomAttribute<FrostySdk.Attributes.EditorAttribute>() != null)
                        subItem.TypeEditor = attributes.GetCustomAttribute<FrostySdk.Attributes.EditorAttribute>().EditorType;
                    if (attributes.GetCustomAttribute<IsExpandedByDefaultAttribute>() != null)
                        subItem.IsExpanded = true;
                    if (attributes.GetCustomAttribute<FixedSizeArrayAttribute>() != null)
                        subItem.IsEnabled = false;

                    subItem.MetaData.AddRange(attributes.GetCustomAttributes<EditorMetaDataAttribute>());
                    subItem.Attributes.AddRange(attributes.GetCustomAttributes<Attribute>());

                    categories[category].Children.Add(subItem);
                    rootChild.Children.Add(subItem);
                }
            }

            return categories.Values.ToArray();
        }

        private void SubItem_PreModified(object sender, ItemPreModifiedEventArgs e)
        {
            OnPreModifiedCommand?.Execute(e);
            OnPreModified?.Invoke(sender, e);
        }

        private void SubItem_Modified(object sender, ItemModifiedEventArgs e)
        {
            if (additionalData != null)
            {
                additionalData.Save(e);
            }

            if (asset != null)
            {
                // @hack: To ensure that changes to only the transient id field are not exported to mods, but
                //        are saved to projects. This allows users to modify ids to their hearts contents without
                //        bloating their mods with unintentional edits.

                asset.TransientEdit = e.Item.GetCustomAttribute<IsTransientAttribute>() != null && e.Item.Name.Equals("__Id");
            }

            Modified = true;

            OnModifiedCommand?.Execute(e);
            OnModified?.Invoke(sender, e);
        }
    }
}
