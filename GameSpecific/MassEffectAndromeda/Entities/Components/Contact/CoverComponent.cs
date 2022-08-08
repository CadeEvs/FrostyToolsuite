using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverComponentData))]
	public class CoverComponent : GameComponent, IEntityData<FrostySdk.Ebx.CoverComponentData>
	{
		public new FrostySdk.Ebx.CoverComponentData Data => data as FrostySdk.Ebx.CoverComponentData;
		public override string DisplayName => "CoverComponent";

		public CoverComponent(FrostySdk.Ebx.CoverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

