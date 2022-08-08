using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlapComponentData))]
	public class FlapComponent : BoneComponent, IEntityData<FrostySdk.Ebx.FlapComponentData>
	{
		public new FrostySdk.Ebx.FlapComponentData Data => data as FrostySdk.Ebx.FlapComponentData;
		public override string DisplayName => "FlapComponent";

		public FlapComponent(FrostySdk.Ebx.FlapComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

