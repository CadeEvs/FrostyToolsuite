
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FaceposerComponentData))]
	public class FaceposerComponent : GameComponent, IEntityData<FrostySdk.Ebx.FaceposerComponentData>
	{
		public new FrostySdk.Ebx.FaceposerComponentData Data => data as FrostySdk.Ebx.FaceposerComponentData;
		public override string DisplayName => "FaceposerComponent";

		public FaceposerComponent(FrostySdk.Ebx.FaceposerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

