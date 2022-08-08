using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWAudioComponentData))]
	public class BWAudioComponent : AudioOwnerComponent, IEntityData<FrostySdk.Ebx.BWAudioComponentData>
	{
		public new FrostySdk.Ebx.BWAudioComponentData Data => data as FrostySdk.Ebx.BWAudioComponentData;
		public override string DisplayName => "BWAudioComponent";

		public BWAudioComponent(FrostySdk.Ebx.BWAudioComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

