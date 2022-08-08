using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsTerrainUpdaterComponentData))]
	public class PhysicsTerrainUpdaterComponent : Component, IEntityData<FrostySdk.Ebx.PhysicsTerrainUpdaterComponentData>
	{
		public new FrostySdk.Ebx.PhysicsTerrainUpdaterComponentData Data => data as FrostySdk.Ebx.PhysicsTerrainUpdaterComponentData;
		public override string DisplayName => "PhysicsTerrainUpdaterComponent";

		public PhysicsTerrainUpdaterComponent(FrostySdk.Ebx.PhysicsTerrainUpdaterComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

