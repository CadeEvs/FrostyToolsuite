using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIButtonLegendActionMapping = FrostySdk.Ebx.UIButtonLegendActionMapping;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UIButtonLegendActionMappings))]
    public class UIButtonLegendActionMappings : Asset, IAssetData<FrostySdk.Ebx.UIButtonLegendActionMappings>
    {
        public FrostySdk.Ebx.UIButtonLegendActionMappings Data => data as FrostySdk.Ebx.UIButtonLegendActionMappings;

        public UIButtonLegendActionMappings(Guid fileGuid, FrostySdk.Ebx.UIButtonLegendActionMappings inData)
            : base(fileGuid, inData)
        {
        }

        public UIButtonLegendActionMapping GetActionMapping(IEnumerable<int> inputConcepts)
        {
            foreach (var mapping in Data.Mappings)
            {
                if (mapping.InputConcepts.SequenceEqual(inputConcepts))
                {
                    return mapping;
                }
            }
            return null;
        }
    }
}
