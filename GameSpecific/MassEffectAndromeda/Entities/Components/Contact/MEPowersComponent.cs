using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPowersComponentData))]
	public class MEPowersComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEPowersComponentData>
	{
		public new FrostySdk.Ebx.MEPowersComponentData Data => data as FrostySdk.Ebx.MEPowersComponentData;
		public override string DisplayName => "MEPowersComponent";

		public MEPowersComponent(FrostySdk.Ebx.MEPowersComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

