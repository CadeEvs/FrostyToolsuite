
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameComponentData))]
	public class GameComponent : Component, IEntityData<FrostySdk.Ebx.GameComponentData>
	{
		public new FrostySdk.Ebx.GameComponentData Data => data as FrostySdk.Ebx.GameComponentData;
		public override string DisplayName => "GameComponent";

		public GameComponent(FrostySdk.Ebx.GameComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

