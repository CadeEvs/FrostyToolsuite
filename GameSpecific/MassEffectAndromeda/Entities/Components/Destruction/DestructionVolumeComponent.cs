using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionVolumeComponentData))]
	public class DestructionVolumeComponent : GameComponent, IEntityData<FrostySdk.Ebx.DestructionVolumeComponentData>
	{
		public new FrostySdk.Ebx.DestructionVolumeComponentData Data => data as FrostySdk.Ebx.DestructionVolumeComponentData;
		public override string DisplayName => "DestructionVolumeComponent";

		public DestructionVolumeComponent(FrostySdk.Ebx.DestructionVolumeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

