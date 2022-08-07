using Frosty.Core.Viewport;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Render
{
    public class BaseRenderable : MeshRenderBase
    {
        public override void Render(DeviceContext context, MeshRenderPath renderPath)
        {
            throw new NotImplementedException();
        }
    }
}
