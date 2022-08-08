using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsDrivenComponentData))]
	public class PhysicsDrivenComponent : GameComponent, IEntityData<FrostySdk.Ebx.PhysicsDrivenComponentData>
	{
		public new FrostySdk.Ebx.PhysicsDrivenComponentData Data => data as FrostySdk.Ebx.PhysicsDrivenComponentData;
		public override string DisplayName => "PhysicsDrivenComponent";

		public PhysicsDrivenComponent(FrostySdk.Ebx.PhysicsDrivenComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

