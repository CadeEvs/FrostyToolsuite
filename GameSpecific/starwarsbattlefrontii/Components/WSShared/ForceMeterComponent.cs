
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceMeterComponentData))]
	public class ForceMeterComponent : GameComponent, IEntityData<FrostySdk.Ebx.ForceMeterComponentData>
	{
		public new FrostySdk.Ebx.ForceMeterComponentData Data => data as FrostySdk.Ebx.ForceMeterComponentData;
		public override string DisplayName => "ForceMeterComponent";

		public ForceMeterComponent(FrostySdk.Ebx.ForceMeterComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

