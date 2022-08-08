
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AILocoComponentData))]
	public class AILocoComponent : GameComponent, IEntityData<FrostySdk.Ebx.AILocoComponentData>
	{
		public new FrostySdk.Ebx.AILocoComponentData Data => data as FrostySdk.Ebx.AILocoComponentData;
		public override string DisplayName => "AILocoComponent";

		public AILocoComponent(FrostySdk.Ebx.AILocoComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

