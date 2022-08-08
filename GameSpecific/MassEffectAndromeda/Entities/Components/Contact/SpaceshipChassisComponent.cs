using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceshipChassisComponentData))]
	public class SpaceshipChassisComponent : MEChassisComponent, IEntityData<FrostySdk.Ebx.SpaceshipChassisComponentData>
	{
		public new FrostySdk.Ebx.SpaceshipChassisComponentData Data => data as FrostySdk.Ebx.SpaceshipChassisComponentData;
		public override string DisplayName => "SpaceshipChassisComponent";

		public SpaceshipChassisComponent(FrostySdk.Ebx.SpaceshipChassisComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

