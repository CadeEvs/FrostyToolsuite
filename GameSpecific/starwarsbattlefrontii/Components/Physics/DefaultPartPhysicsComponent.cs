
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DefaultPartPhysicsComponentData))]
	public class DefaultPartPhysicsComponent : PartPhysicsComponent, IEntityData<FrostySdk.Ebx.DefaultPartPhysicsComponentData>
	{
		public new FrostySdk.Ebx.DefaultPartPhysicsComponentData Data => data as FrostySdk.Ebx.DefaultPartPhysicsComponentData;
		public override string DisplayName => "DefaultPartPhysicsComponent";

		public DefaultPartPhysicsComponent(FrostySdk.Ebx.DefaultPartPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

