
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoxReflectionVolumeComponentData))]
	public class BoxReflectionVolumeComponent : ReflectionVolumeComponent, IEntityData<FrostySdk.Ebx.BoxReflectionVolumeComponentData>
	{
		public new FrostySdk.Ebx.BoxReflectionVolumeComponentData Data => data as FrostySdk.Ebx.BoxReflectionVolumeComponentData;
		public override string DisplayName => "BoxReflectionVolumeComponent";

		public BoxReflectionVolumeComponent(FrostySdk.Ebx.BoxReflectionVolumeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

