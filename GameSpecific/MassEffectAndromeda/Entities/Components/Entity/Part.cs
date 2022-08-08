using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartData))]
	public class Part : Component, IEntityData<FrostySdk.Ebx.PartData>
	{
		public new FrostySdk.Ebx.PartData Data => data as FrostySdk.Ebx.PartData;
		public override string DisplayName => "Part";

		public Part(FrostySdk.Ebx.PartData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

