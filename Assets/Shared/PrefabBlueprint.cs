using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    public class PrefabBlueprint : Blueprint, IAssetData<FrostySdk.Ebx.PrefabBlueprint>
    {
        public new FrostySdk.Ebx.PrefabBlueprint Data => data as FrostySdk.Ebx.PrefabBlueprint;
        public List<FrostySdk.Ebx.PropertyConnection> PropertyConnections { get; protected set; } = new List<FrostySdk.Ebx.PropertyConnection>();
        public List<FrostySdk.Ebx.EventConnection> EventConnections { get; protected set; } = new List<FrostySdk.Ebx.EventConnection>();
        public List<FrostySdk.Ebx.LinkConnection> LinkConnections { get; protected set; } = new List<FrostySdk.Ebx.LinkConnection>();

        public PrefabBlueprint(Guid fileGuid, FrostySdk.Ebx.PrefabBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
