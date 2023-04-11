using System;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Sdk;

namespace FrostyTypeSdkGenerator.Example
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AssetData asset1 = new();
            AssetData asset2 = new();

            string name = asset1.Name;
            asset2.Count = 2;
            asset1.SetCount(1);
            asset2.SetName("asset2");
            
            Console.WriteLine(asset1.Equals(asset2));

            DataContainer dataContainer = new();
            dataContainer.GetInstanceGuid();
        }
    }

    // testing some classes and structs
    
    public partial struct AssetData
    {
        [EbxFieldMeta(TypeFlags.TypeEnum.CString, 0u)]
        private Frosty.Sdk.Ebx.CString _Name;
        private int _Count;
        private bool _IsEnabled;

        public void SetName(string name) => _Name = name;
        public void SetCount(int count) => _Count = count;
        public void SetIsEnabled(bool isEnabled) => _IsEnabled = isEnabled;
    }

    public partial class DataContainer
    {
    }

    public partial class Asset : DataContainer
    {
        private string _Name;
        private string GetId() => _Name;
    }

    public partial class DataContainerAsset : DataContainer
    {
        private string _Name;
        private string GetId() => _Name;
    }
}