using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESpaceToolComponentData))]
	public class MESpaceToolComponent : MEPowersComponent, IEntityData<FrostySdk.Ebx.MESpaceToolComponentData>
	{
		public new FrostySdk.Ebx.MESpaceToolComponentData Data => data as FrostySdk.Ebx.MESpaceToolComponentData;
		public override string DisplayName => "MESpaceToolComponent";

		public MESpaceToolComponent(FrostySdk.Ebx.MESpaceToolComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

