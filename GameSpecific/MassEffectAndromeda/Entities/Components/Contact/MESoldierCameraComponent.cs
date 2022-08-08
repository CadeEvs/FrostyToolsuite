using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierCameraComponentData))]
	public class MESoldierCameraComponent : SoldierCameraComponent, IEntityData<FrostySdk.Ebx.MESoldierCameraComponentData>
	{
		public new FrostySdk.Ebx.MESoldierCameraComponentData Data => data as FrostySdk.Ebx.MESoldierCameraComponentData;
		public override string DisplayName => "MESoldierCameraComponent";

		public MESoldierCameraComponent(FrostySdk.Ebx.MESoldierCameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

