
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EngineComponentData))]
	public class EngineComponent : GameComponent, IEntityData<FrostySdk.Ebx.EngineComponentData>
	{
		public new FrostySdk.Ebx.EngineComponentData Data => data as FrostySdk.Ebx.EngineComponentData;
		public override string DisplayName => "EngineComponent";

		public EngineComponent(FrostySdk.Ebx.EngineComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

