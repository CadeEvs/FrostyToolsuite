
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SphereReflectionVolumeComponentData))]
	public class SphereReflectionVolumeComponent : ReflectionVolumeComponent, IEntityData<FrostySdk.Ebx.SphereReflectionVolumeComponentData>
	{
		public new FrostySdk.Ebx.SphereReflectionVolumeComponentData Data => data as FrostySdk.Ebx.SphereReflectionVolumeComponentData;
		public override string DisplayName => "SphereReflectionVolumeComponent";

		public SphereReflectionVolumeComponent(FrostySdk.Ebx.SphereReflectionVolumeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

