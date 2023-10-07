using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Frosty.Core.Controls
{
    // ...
    // Clipboard
    // ...

    public class FrostyClipboard : INotifyPropertyChanged
    {
        private static FrostyClipboard _instance;
        private FrostyClipboard() { }
        public static FrostyClipboard Current => _instance ?? (_instance = new FrostyClipboard());

        public object Data;

        public bool HasData { get => Data != null; set { } }
        public void SetData(object data)
        {
            Type dataType = data.GetType();
            Dictionary<object, object> oldNewMapping = new Dictionary<object, object>();

            Data = data;
            //Data = CopyValue(data, null, null, ref oldNewMapping);
            RaisePropertyChanged("HasData");
            RaisePropertyChanged("HasPointerRef");
        }
        public bool IsType(Type type)
        {
            return (Data != null && Data.GetType() == type);
        }
        public object GetData(EbxAsset asset, EbxAssetEntry entry, PasteType type = PasteType.Deep)
        {
            Dictionary<object, object> oldNewMapping = new Dictionary<object, object>();
            object copyOfData = CopyValue(Data, asset, entry, ref oldNewMapping, type, true);
            return copyOfData;
        }
        public object GetData()
        {
            Dictionary<object, object> oldNewMapping = new Dictionary<object, object>();
            object copyOfData = CopyValue(Data, null, null, ref oldNewMapping);
            return copyOfData;
        }

        public enum PasteType
        {
            Deep,
            Shallow,
            PR,
        };

        public void Clear()
        {
            Data = null;
        }

        public bool HasPointerRef { get => Data is PointerRef; set { } }

        private List<string> connections = new List<string>() { "PropertyConnection", "EventConnection", "LinkConnection" };


        private object DeepCopy(object data, EbxAsset asset, EbxAssetEntry entry, ref Dictionary<object, object> oldNewMapping, PasteType pasteType = PasteType.Deep)
        {
            Type dataType = data.GetType();
            if (dataType.IsPrimitive || dataType.IsValueType)
                return data;

            dynamic newData;
            if (dataType.GetCustomAttribute<EbxClassMetaAttribute>().Type == EbxFieldType.Pointer)
            {
                if (oldNewMapping.ContainsKey(data))
                    return oldNewMapping[data];

                newData = TypeLibrary.CreateObject(dataType.Name);
                oldNewMapping.Add(data, newData);

                AssetClassGuid guid = ((dynamic)data).GetInstanceGuid();
                if (guid.IsExported)
                {
                    if (asset != null)
                    {
                        Type t = newData.GetType();
                        guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(asset.Objects, t, entry.Guid), -1);
                        newData.SetInstanceGuid(guid);
                    }
                    else
                    {
                        guid = new AssetClassGuid(Guid.NewGuid(), -1);
                        newData.SetInstanceGuid(guid);
                    }
                }
                else
                {
                    guid = new AssetClassGuid(-1);
                    newData.SetInstanceGuid(guid);
                }

                if (asset != null)
                    asset.AddObject(newData);
            }
            else
            {
                newData = TypeLibrary.CreateObject(dataType.Name);
                // the type most likely comes from a plugin if CreateObject returns null, so just create a new instance here
                if (newData == null)
                {
                    newData = Activator.CreateInstance(dataType);
                }
            }
            foreach (PropertyInfo pi in dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (pi.Name.StartsWith("__"))
                    continue;

                dynamic oldValue = pi.GetValue(data);
                pi.SetValue(newData, CopyValue(oldValue, asset, entry, ref oldNewMapping, pasteType));
            }
            if (connections.Contains(dataType.Name))
            {
                PointerRef oldSource = (PointerRef)((dynamic)data).Source;
                PointerRef oldTarget = (PointerRef)((dynamic)data).Target;

                if (oldSource.Type == PointerRefType.External && !asset.Objects.Contains(oldSource.External) || oldSource.Type == PointerRefType.Internal && asset.Objects.Contains(oldSource.Internal))
                    newData.Source = oldSource;

                if (oldTarget.Type == PointerRefType.External && !asset.Objects.Contains(oldTarget.External) || oldTarget.Type == PointerRefType.Internal && asset.Objects.Contains(oldTarget.Internal))
                    newData.Target = oldTarget;
            }
            return newData;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private object CopyValue(object obj, EbxAsset asset, EbxAssetEntry entry, ref Dictionary<object, object> oldNewMapping, PasteType pasteType = PasteType.Deep, bool start = false)
        {
            Type objType = obj.GetType();
            if (objType == typeof(PointerRef))
            {
                PointerRef currentPr = (PointerRef)obj;
                PointerRef newPr;

                switch (currentPr.Type)
                {
                    case PointerRefType.External:
                        newPr = new PointerRef(currentPr.External);
                        break;
                    case PointerRefType.Internal:
                        {
                            if (pasteType == PasteType.Deep || (pasteType == PasteType.Shallow && start))
                            {
                                dynamic newObj = DeepCopy(currentPr.Internal, asset, entry, ref oldNewMapping, pasteType);

                                newPr = new PointerRef(newObj);
                                //AssetClassGuid guid = ((dynamic)currentPr.Internal).GetInstanceGuid();

                                //if (guid.IsExported)
                                //{
                                //    if (asset != null)
                                //    {
                                //        guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(asset.Objects, newPr.Internal.GetType().Name, entry.Guid), -1);
                                //        ((dynamic)newPr.Internal).SetInstanceGuid(guid);
                                //    }
                                //    else
                                //    {
                                //        guid = new AssetClassGuid(Guid.NewGuid(), -1);
                                //        ((dynamic)newPr.Internal).SetInstanceGuid(guid);
                                //    }
                                //}
                                //else
                                //{
                                //    guid = new AssetClassGuid(-1);
                                //    ((dynamic)newPr.Internal).SetInstanceGuid(guid);
                                //}
                            }
                            else
                            {
                                if (!asset.Objects.Contains(currentPr.Internal))
                                {
                                    App.Logger.LogError("Could not copy pointer reference to object which does not exist in this instance");
                                    return null;
                                }
                                newPr = new PointerRef(currentPr.Internal);
                            }
                        }
                        break;
                    default:
                        newPr = new PointerRef();
                        break;
                }
                return newPr;
            }

            if (objType.GetInterface("IList") != null)
            {
                IList oldList = (IList)obj;
                IList newList = (IList)Activator.CreateInstance(objType);

                newList.Clear();
                for (int i = 0; i < oldList.Count; i++)
                    newList.Add(CopyValue(oldList[i], asset, entry, ref oldNewMapping, pasteType));

                return newList;
            }

            return DeepCopy(obj, asset, entry, ref oldNewMapping, pasteType);
        }
    }
}
