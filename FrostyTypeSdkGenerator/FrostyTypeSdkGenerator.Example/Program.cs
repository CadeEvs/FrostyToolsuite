using System;
using System.Collections.Generic;
using System.Linq;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Ebx;
using Frosty.Sdk.Sdk;

namespace FrostyTypeSdkGenerator.Example
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            List<int> test1 = new()
            {
                1, 2, 3
            };
            
            List<int> test2 = new()
            {
                1, 2, 3
            };

            AssetData data1 = new();
            AssetData data2 = new();

            Asset asset = new();
            //asset.SetInstanceGuid(new AssetClassGuid());
            
            Console.WriteLine(data1.Equals(data2));
        }
    }

    // testing some classes and structs
    
    public partial struct AssetData
    {
        [EbxFieldMeta(TypeFlags.TypeEnum.CString, 0u)]
        private Frosty.Sdk.Ebx.CString _Name;
        private int _Count;
        private bool _IsEnabled;

        private List<int> _SomeList;

        public void SetName(string name) => _Name = name;
        public void SetCount(int count) => _Count = count;
        public void SetIsEnabled(bool isEnabled) => _IsEnabled = isEnabled;
    }

    
}