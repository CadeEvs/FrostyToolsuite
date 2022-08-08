using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntentsComponentData))]
	public class IntentsComponent : InputTreeBaseComponent, IEntityData<FrostySdk.Ebx.IntentsComponentData>
	{
		public new FrostySdk.Ebx.IntentsComponentData Data => data as FrostySdk.Ebx.IntentsComponentData;
		public override string DisplayName => "IntentsComponent";

		public IntentsComponent(FrostySdk.Ebx.IntentsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

