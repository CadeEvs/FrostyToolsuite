
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MorphComponentData))]
	public class MorphComponent : GameComponent, IEntityData<FrostySdk.Ebx.MorphComponentData>
	{
		public new FrostySdk.Ebx.MorphComponentData Data => data as FrostySdk.Ebx.MorphComponentData;
		public override string DisplayName => "MorphComponent";

		public MorphComponent(FrostySdk.Ebx.MorphComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

