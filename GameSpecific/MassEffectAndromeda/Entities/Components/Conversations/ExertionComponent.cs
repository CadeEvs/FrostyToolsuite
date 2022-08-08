using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExertionComponentData))]
	public class ExertionComponent : GameComponent, IEntityData<FrostySdk.Ebx.ExertionComponentData>
	{
		public new FrostySdk.Ebx.ExertionComponentData Data => data as FrostySdk.Ebx.ExertionComponentData;
		public override string DisplayName => "ExertionComponent";

		public ExertionComponent(FrostySdk.Ebx.ExertionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

