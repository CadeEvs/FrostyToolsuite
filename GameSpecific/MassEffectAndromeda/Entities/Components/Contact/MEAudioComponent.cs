using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAudioComponentData))]
	public class MEAudioComponent : BWAudioComponent, IEntityData<FrostySdk.Ebx.MEAudioComponentData>
	{
		public new FrostySdk.Ebx.MEAudioComponentData Data => data as FrostySdk.Ebx.MEAudioComponentData;
		public override string DisplayName => "MEAudioComponent";

		public MEAudioComponent(FrostySdk.Ebx.MEAudioComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

