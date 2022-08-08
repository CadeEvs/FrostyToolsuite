using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoneComponentData))]
	public class BoneComponent : GameComponent, IEntityData<FrostySdk.Ebx.BoneComponentData>
	{
		public new FrostySdk.Ebx.BoneComponentData Data => data as FrostySdk.Ebx.BoneComponentData;
		public override string DisplayName => "BoneComponent";

		public BoneComponent(FrostySdk.Ebx.BoneComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

