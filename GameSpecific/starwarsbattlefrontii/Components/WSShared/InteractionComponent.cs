
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InteractionComponentData))]
	public class InteractionComponent : GameComponent, IEntityData<FrostySdk.Ebx.InteractionComponentData>
	{
		public new FrostySdk.Ebx.InteractionComponentData Data => data as FrostySdk.Ebx.InteractionComponentData;
		public override string DisplayName => "InteractionComponent";

		public InteractionComponent(FrostySdk.Ebx.InteractionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

