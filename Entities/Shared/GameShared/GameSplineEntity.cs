using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core.Viewport;
using FrostySdk;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;
using D3D11 = SharpDX.Direct3D11;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.GameSplineEntityData))]
    public class GameSplineEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.GameSplineEntityData>
    {
        public new FrostySdk.Ebx.GameSplineEntityData Data => data as FrostySdk.Ebx.GameSplineEntityData;

        public GameSplineEntity(FrostySdk.Ebx.GameSplineEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }
    }
}
