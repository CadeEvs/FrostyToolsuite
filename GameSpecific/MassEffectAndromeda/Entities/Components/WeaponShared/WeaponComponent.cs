using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponComponentData))]
	public class WeaponComponent : BoneComponent, IEntityData<FrostySdk.Ebx.WeaponComponentData>
	{
		public new FrostySdk.Ebx.WeaponComponentData Data => data as FrostySdk.Ebx.WeaponComponentData;
		public override string DisplayName => "WeaponComponent";

		public WeaponComponent(FrostySdk.Ebx.WeaponComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

