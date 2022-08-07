using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintSpawnReferenceObjectData))]
    public class BlueprintSpawnReferenceObject : ReferenceObject, IEntityData<FrostySdk.Ebx.BlueprintSpawnReferenceObjectData>
    {
        public new FrostySdk.Ebx.BlueprintSpawnReferenceObjectData Data => data as FrostySdk.Ebx.BlueprintSpawnReferenceObjectData;

        public BlueprintSpawnReferenceObject(FrostySdk.Ebx.BlueprintSpawnReferenceObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
        }
    }
}
