using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemnantInterfaceObjectComponentData))]
	public class RemnantInterfaceObjectComponent : GameComponent, IEntityData<FrostySdk.Ebx.RemnantInterfaceObjectComponentData>
	{
		public new FrostySdk.Ebx.RemnantInterfaceObjectComponentData Data => data as FrostySdk.Ebx.RemnantInterfaceObjectComponentData;
		public override string DisplayName => "RemnantInterfaceObjectComponent";

		public RemnantInterfaceObjectComponent(FrostySdk.Ebx.RemnantInterfaceObjectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

