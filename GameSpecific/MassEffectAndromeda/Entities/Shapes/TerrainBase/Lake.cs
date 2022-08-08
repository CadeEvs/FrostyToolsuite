using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LakeData))]
	public class Lake : VisualVectorShape, IEntityData<FrostySdk.Ebx.LakeData>
	{
		public new FrostySdk.Ebx.LakeData Data => data as FrostySdk.Ebx.LakeData;
		public override string DisplayName => "Lake";

		public Lake(FrostySdk.Ebx.LakeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

