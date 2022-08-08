using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DriverComponentData))]
	public class DriverComponent : GameComponent, IEntityData<FrostySdk.Ebx.DriverComponentData>
	{
		public new FrostySdk.Ebx.DriverComponentData Data => data as FrostySdk.Ebx.DriverComponentData;
		public override string DisplayName => "DriverComponent";

		public DriverComponent(FrostySdk.Ebx.DriverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

