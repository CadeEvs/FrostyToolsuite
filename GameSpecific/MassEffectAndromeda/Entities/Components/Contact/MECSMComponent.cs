using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECSMComponentData))]
	public class MECSMComponent : BWCSMComponent, IEntityData<FrostySdk.Ebx.MECSMComponentData>
	{
		public new FrostySdk.Ebx.MECSMComponentData Data => data as FrostySdk.Ebx.MECSMComponentData;
		public override string DisplayName => "MECSMComponent";

		public MECSMComponent(FrostySdk.Ebx.MECSMComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

