using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEReactionsComponentData))]
	public class MEReactionsComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEReactionsComponentData>
	{
		public new FrostySdk.Ebx.MEReactionsComponentData Data => data as FrostySdk.Ebx.MEReactionsComponentData;
		public override string DisplayName => "MEReactionsComponent";

		public MEReactionsComponent(FrostySdk.Ebx.MEReactionsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

