using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundsetComponentData))]
	public class SoundsetComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoundsetComponentData>
	{
		public new FrostySdk.Ebx.SoundsetComponentData Data => data as FrostySdk.Ebx.SoundsetComponentData;
		public override string DisplayName => "SoundsetComponent";

		public SoundsetComponent(FrostySdk.Ebx.SoundsetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

