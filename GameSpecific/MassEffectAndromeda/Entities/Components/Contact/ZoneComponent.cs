using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneComponentData))]
	public class ZoneComponent : GameComponent, IEntityData<FrostySdk.Ebx.ZoneComponentData>
	{
		public new FrostySdk.Ebx.ZoneComponentData Data => data as FrostySdk.Ebx.ZoneComponentData;
		public override string DisplayName => "ZoneComponent";

		public ZoneComponent(FrostySdk.Ebx.ZoneComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

