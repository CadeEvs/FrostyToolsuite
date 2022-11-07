using Frosty.Core.Viewport;
using MeshSetPlugin.Resources;
using SharpDX;
using System;
using System.Collections.Generic;

namespace MeshSetPlugin.Render
{
    public class MeshRenderMesh : IDisposable
    {
        public BoundingBox Bounds;
        public IEnumerable<MeshRenderLod> LODs => lods;
        private List<MeshRenderLod> lods = new List<MeshRenderLod>();

        public MeshRenderMesh(RenderCreateState state, MeshSet meshSet, MeshMaterialCollection materials, MeshRenderSkeleton skeleton)
        {
            foreach (MeshSetLod lod in meshSet.Lods)
            {
                MeshRenderLod renderLod = new MeshRenderLod(state, lod, materials, skeleton);
                lods.Add(renderLod);
            }

            Bounds = new BoundingBox(
                new Vector3(meshSet.BoundingBox.min.x, meshSet.BoundingBox.min.y, meshSet.BoundingBox.min.z),
                new Vector3(meshSet.BoundingBox.max.x, meshSet.BoundingBox.max.y, meshSet.BoundingBox.max.z)
                );
        }

        public void SetMaterials(RenderCreateState state, MeshMaterialCollection materials)
        {
            foreach (MeshRenderLod lod in lods)
            {
                lod.SetMaterials(state, materials);
            }
        }

        public MeshRenderLod GetLod(int idx)
        {
            if (idx >= lods.Count)
            {
                return lods[lods.Count - 1];
            }

            return lods[idx];
        }

        public void Dispose()
        {
            foreach (MeshRenderLod lod in lods)
            {
                lod.Dispose();
            }
        }
    }
}
