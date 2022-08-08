using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SensingComponentData))]
	public class SensingComponent : GameComponent, IEntityData<FrostySdk.Ebx.SensingComponentData>
	{
		public new FrostySdk.Ebx.SensingComponentData Data => data as FrostySdk.Ebx.SensingComponentData;
		public override string DisplayName => "SensingComponent";

		public SensingComponent(FrostySdk.Ebx.SensingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

