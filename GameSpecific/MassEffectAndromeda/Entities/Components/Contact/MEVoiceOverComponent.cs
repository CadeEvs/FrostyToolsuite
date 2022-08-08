using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEVoiceOverComponentData))]
	public class MEVoiceOverComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEVoiceOverComponentData>
	{
		public new FrostySdk.Ebx.MEVoiceOverComponentData Data => data as FrostySdk.Ebx.MEVoiceOverComponentData;
		public override string DisplayName => "MEVoiceOverComponent";

		public MEVoiceOverComponent(FrostySdk.Ebx.MEVoiceOverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

