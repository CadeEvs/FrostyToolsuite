
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIBucketSystemComponentData))]
	public class AIBucketSystemComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIBucketSystemComponentData>
	{
		public new FrostySdk.Ebx.AIBucketSystemComponentData Data => data as FrostySdk.Ebx.AIBucketSystemComponentData;
		public override string DisplayName => "AIBucketSystemComponent";

		public AIBucketSystemComponent(FrostySdk.Ebx.AIBucketSystemComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

