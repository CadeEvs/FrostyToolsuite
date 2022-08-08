using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraRelativeInputComponentData))]
	public class CameraRelativeInputComponent : GameComponent, IEntityData<FrostySdk.Ebx.CameraRelativeInputComponentData>
	{
		public new FrostySdk.Ebx.CameraRelativeInputComponentData Data => data as FrostySdk.Ebx.CameraRelativeInputComponentData;
		public override string DisplayName => "CameraRelativeInputComponent";

		public CameraRelativeInputComponent(FrostySdk.Ebx.CameraRelativeInputComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

