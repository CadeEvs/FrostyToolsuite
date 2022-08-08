using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemnantInterfacePlayerComponentData))]
	public class RemnantInterfacePlayerComponent : GameComponent, IEntityData<FrostySdk.Ebx.RemnantInterfacePlayerComponentData>
	{
		public new FrostySdk.Ebx.RemnantInterfacePlayerComponentData Data => data as FrostySdk.Ebx.RemnantInterfacePlayerComponentData;
		public override string DisplayName => "RemnantInterfacePlayerComponent";

		public RemnantInterfacePlayerComponent(FrostySdk.Ebx.RemnantInterfacePlayerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

