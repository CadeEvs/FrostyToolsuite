using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        private object Data;

        public bool HasData { get => Data != null; set { } }
        public void SetData(object data)
        {
            Type dataType = data.GetType();
            Dictionary<object, object> oldNewMapping = new Dictionary<object, object>();

            Data = DeepCopyValue(data, null, null, ref oldNewMapping);
            RaisePropertyChanged("HasData");
        }
        public bool IsType(Type type)
        {
            return (Data != null && Data.GetType() == type);
        }
        public object GetData(EbxAsset asset, EbxAssetEntry entry)
        {
            Dictionary<object, object> oldNewMapping = new Dictionary<object, object>();
            object copyOfData = DeepCopyValue(Data, asset, entry, ref oldNewMapping);
            return copyOfData;
        }
        public void Clear()
        {
            Data = null;
        }

        private object DeepCopy(object data, EbxAsset asset, EbxAssetEntry entry, ref Dictionary<object, object> oldNewMapping)
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
            }

            foreach (PropertyInfo pi in dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (pi.Name.StartsWith("__"))
                    continue;

                dynamic oldValue = pi.GetValue(data);
                pi.SetValue(newData, DeepCopyValue(oldValue, asset, entry, ref oldNewMapping));
            }
            return newData;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private object DeepCopyValue(object obj, EbxAsset asset, EbxAssetEntry entry, ref Dictionary<object, object> oldNewMapping)
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
                            dynamic newObj = DeepCopy(currentPr.Internal, asset, entry, ref oldNewMapping);

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
                    newList.Add(DeepCopyValue(oldList[i], asset, entry, ref oldNewMapping));

                return newList;
            }

            return DeepCopy(obj, asset, entry, ref oldNewMapping);
        }
    }
}
