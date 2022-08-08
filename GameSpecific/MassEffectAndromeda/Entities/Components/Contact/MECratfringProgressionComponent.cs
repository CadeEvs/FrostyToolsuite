using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECratfringProgressionComponentData))]
	public class MECratfringProgressionComponent : GameComponent, IEntityData<FrostySdk.Ebx.MECratfringProgressionComponentData>
	{
		public new FrostySdk.Ebx.MECratfringProgressionComponentData Data => data as FrostySdk.Ebx.MECratfringProgressionComponentData;
		public override string DisplayName => "MECratfringProgressionComponent";

		public MECratfringProgressionComponent(FrostySdk.Ebx.MECratfringProgressionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

