using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NaturalTerrainCoverComponentData))]
	public class NaturalTerrainCoverComponent : CoverComponent, IEntityData<FrostySdk.Ebx.NaturalTerrainCoverComponentData>
	{
		public new FrostySdk.Ebx.NaturalTerrainCoverComponentData Data => data as FrostySdk.Ebx.NaturalTerrainCoverComponentData;
		public override string DisplayName => "NaturalTerrainCoverComponent";

		public NaturalTerrainCoverComponent(FrostySdk.Ebx.NaturalTerrainCoverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

