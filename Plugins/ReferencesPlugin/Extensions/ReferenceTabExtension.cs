using Frosty.Controls;
using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReferencesPlugin.Extensions
{
    public class ReferenceTabExtension : TabExtension
    {
        public override string TabItemName => "References";

        public override FrostyTabItem TabContent => new ReferenceTabItem();
    }
}
