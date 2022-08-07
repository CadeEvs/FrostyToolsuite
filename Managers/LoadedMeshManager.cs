using Frosty.Core.Viewport;
using LevelEditorPlugin.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Managers
{
    public class LoadedMeshManager
    {
        #region -- Singleton --

        public static LoadedMeshManager Instance { get; private set; } = new LoadedMeshManager();
        private LoadedMeshManager() { }

        #endregion

        private class LoadedMeshInfo
        {
            public ObjRenderable Mesh;
            public int RefCount;

            public void Increment() { RefCount++; }
            public void Decrement() { RefCount--; }
        }

        private Dictionary<uint, LoadedMeshInfo> loadedMeshes = new Dictionary<uint, LoadedMeshInfo>();

        public ObjRenderable LoadMesh(RenderCreateState state, string name)
        {
            uint hash = (uint)Frosty.Hash.Fnv1.HashString(name.ToLower());
            if (!loadedMeshes.ContainsKey(hash))
            {
                loadedMeshes.Add(hash, new LoadedMeshInfo() { Mesh = new ObjRenderable(state, name) });
            }

            loadedMeshes[hash].Increment();
            return loadedMeshes[hash].Mesh;
        }

        public void UnloadMesh(ObjRenderable mesh)
        {
            var result = loadedMeshes.Where(m => m.Value.Mesh == mesh);
            if (result.Count() == 0)
                return;

            var loadedMesh = result.First();
            loadedMesh.Value.Decrement();

            if (loadedMesh.Value.RefCount == 0)
            {
                loadedMesh.Value.Mesh.Dispose();
                loadedMeshes.Remove(loadedMesh.Key);
            }
        }
    }
}
