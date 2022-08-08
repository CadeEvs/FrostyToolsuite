
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActorCustomizationComponentData))]
	public class ActorCustomizationComponent : GameComponent, IEntityData<FrostySdk.Ebx.ActorCustomizationComponentData>
	{
		public new FrostySdk.Ebx.ActorCustomizationComponentData Data => data as FrostySdk.Ebx.ActorCustomizationComponentData;
		public override string DisplayName => "ActorCustomizationComponent";

		public ActorCustomizationComponent(FrostySdk.Ebx.ActorCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

