using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReflectionVolumeComponentData))]
	public class ReflectionVolumeComponent : GameComponent, IEntityData<FrostySdk.Ebx.ReflectionVolumeComponentData>
	{
		public new FrostySdk.Ebx.ReflectionVolumeComponentData Data => data as FrostySdk.Ebx.ReflectionVolumeComponentData;
		public override string DisplayName => "ReflectionVolumeComponent";

		public ReflectionVolumeComponent(FrostySdk.Ebx.ReflectionVolumeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

