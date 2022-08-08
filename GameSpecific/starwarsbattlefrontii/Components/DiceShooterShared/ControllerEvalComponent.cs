
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerEvalComponentData))]
	public class ControllerEvalComponent : GameComponent, IEntityData<FrostySdk.Ebx.ControllerEvalComponentData>
	{
		public new FrostySdk.Ebx.ControllerEvalComponentData Data => data as FrostySdk.Ebx.ControllerEvalComponentData;
		public override string DisplayName => "ControllerEvalComponent";

		public ControllerEvalComponent(FrostySdk.Ebx.ControllerEvalComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

