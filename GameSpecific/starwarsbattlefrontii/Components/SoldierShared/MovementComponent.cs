
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MovementComponentData))]
	public class MovementComponent : GameComponent, IEntityData<FrostySdk.Ebx.MovementComponentData>
	{
		public new FrostySdk.Ebx.MovementComponentData Data => data as FrostySdk.Ebx.MovementComponentData;
		public override string DisplayName => "MovementComponent";

		public MovementComponent(FrostySdk.Ebx.MovementComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

