using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutomatedTurretComponentData))]
	public class AutomatedTurretComponent : GameComponent, IEntityData<FrostySdk.Ebx.AutomatedTurretComponentData>
	{
		public new FrostySdk.Ebx.AutomatedTurretComponentData Data => data as FrostySdk.Ebx.AutomatedTurretComponentData;
		public override string DisplayName => "AutomatedTurretComponent";

		public AutomatedTurretComponent(FrostySdk.Ebx.AutomatedTurretComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

