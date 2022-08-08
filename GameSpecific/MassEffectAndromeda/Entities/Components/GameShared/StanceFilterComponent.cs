using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StanceFilterComponentData))]
	public class StanceFilterComponent : GameComponent, IEntityData<FrostySdk.Ebx.StanceFilterComponentData>
	{
		public new FrostySdk.Ebx.StanceFilterComponentData Data => data as FrostySdk.Ebx.StanceFilterComponentData;
		public override string DisplayName => "StanceFilterComponent";

		public StanceFilterComponent(FrostySdk.Ebx.StanceFilterComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

