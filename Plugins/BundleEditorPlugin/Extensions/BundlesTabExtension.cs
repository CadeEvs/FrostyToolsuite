using Frosty.Controls;
using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BundleEditPlugin
{
    public class BundlesTabExtension : TabExtension
    {
        public override string TabItemName => "Bundles";

        public override FrostyTabItem TabContent => new BundleTabItem();
    }
}
