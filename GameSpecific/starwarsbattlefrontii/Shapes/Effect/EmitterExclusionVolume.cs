using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterExclusionVolumeData))]
	public class EmitterExclusionVolume : OBB, IEntityData<FrostySdk.Ebx.EmitterExclusionVolumeData>
	{
		public new FrostySdk.Ebx.EmitterExclusionVolumeData Data => data as FrostySdk.Ebx.EmitterExclusionVolumeData;
		public override string DisplayName => "EmitterExclusionVolume";

		public EmitterExclusionVolume(FrostySdk.Ebx.EmitterExclusionVolumeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

