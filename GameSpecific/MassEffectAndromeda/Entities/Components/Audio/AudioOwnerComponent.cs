using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioOwnerComponentData))]
	public class AudioOwnerComponent : Component, IEntityData<FrostySdk.Ebx.AudioOwnerComponentData>
	{
		public new FrostySdk.Ebx.AudioOwnerComponentData Data => data as FrostySdk.Ebx.AudioOwnerComponentData;
		public override string DisplayName => "AudioOwnerComponent";

		public AudioOwnerComponent(FrostySdk.Ebx.AudioOwnerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

