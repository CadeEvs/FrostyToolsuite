using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StressComponentData))]
	public class StressComponent : GameComponent, IEntityData<FrostySdk.Ebx.StressComponentData>
	{
		public new FrostySdk.Ebx.StressComponentData Data => data as FrostySdk.Ebx.StressComponentData;
		public override string DisplayName => "StressComponent";

		public StressComponent(FrostySdk.Ebx.StressComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

