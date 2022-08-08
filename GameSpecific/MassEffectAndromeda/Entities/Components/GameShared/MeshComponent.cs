using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshComponentData))]
	public class MeshComponent : GameComponent, IEntityData<FrostySdk.Ebx.MeshComponentData>
	{
		public new FrostySdk.Ebx.MeshComponentData Data => data as FrostySdk.Ebx.MeshComponentData;
		public override string DisplayName => "MeshComponent";
		public Assets.MeshAsset Mesh { get; private set; }

		public MeshComponent(FrostySdk.Ebx.MeshComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
			Mesh = LoadedAssetManager.Instance.LoadAsset<Assets.MeshAsset>(this, Data.Mesh);
		}

		public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
		{
			Mesh.LoadResource(state);
			proxies.Add(new ModelRenderProxy(state, this, Mesh.MeshData));

			SetFlags(EntityFlags.RenderProxyGenerated);
		}

		public override void Destroy()
		{
			LoadedAssetManager.Instance.UnloadAsset(Mesh);
		}
	}
}

