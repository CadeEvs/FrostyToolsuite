using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponZeroingComponentData))]
	public class WeaponZeroingComponent : GameComponent, IEntityData<FrostySdk.Ebx.WeaponZeroingComponentData>
	{
		public new FrostySdk.Ebx.WeaponZeroingComponentData Data => data as FrostySdk.Ebx.WeaponZeroingComponentData;
		public override string DisplayName => "WeaponZeroingComponent";

		public WeaponZeroingComponent(FrostySdk.Ebx.WeaponZeroingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

