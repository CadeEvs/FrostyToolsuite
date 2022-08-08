using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICoverComponentData))]
	public class AICoverComponent : CoverComponent, IEntityData<FrostySdk.Ebx.AICoverComponentData>
	{
		public new FrostySdk.Ebx.AICoverComponentData Data => data as FrostySdk.Ebx.AICoverComponentData;
		public override string DisplayName => "AICoverComponent";

		public AICoverComponent(FrostySdk.Ebx.AICoverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

