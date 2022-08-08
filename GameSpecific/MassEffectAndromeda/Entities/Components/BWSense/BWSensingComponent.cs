using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWSensingComponentData))]
	public class BWSensingComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWSensingComponentData>
	{
		public new FrostySdk.Ebx.BWSensingComponentData Data => data as FrostySdk.Ebx.BWSensingComponentData;
		public override string DisplayName => "BWSensingComponent";

		public BWSensingComponent(FrostySdk.Ebx.BWSensingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

