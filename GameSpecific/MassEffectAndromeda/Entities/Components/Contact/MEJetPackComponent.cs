using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEJetPackComponentData))]
	public class MEJetPackComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEJetPackComponentData>
	{
		public new FrostySdk.Ebx.MEJetPackComponentData Data => data as FrostySdk.Ebx.MEJetPackComponentData;
		public override string DisplayName => "MEJetPackComponent";

		public MEJetPackComponent(FrostySdk.Ebx.MEJetPackComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

