using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SensibleComponentData))]
	public class SensibleComponent : GameComponent, IEntityData<FrostySdk.Ebx.SensibleComponentData>
	{
		public new FrostySdk.Ebx.SensibleComponentData Data => data as FrostySdk.Ebx.SensibleComponentData;
		public override string DisplayName => "SensibleComponent";

		public SensibleComponent(FrostySdk.Ebx.SensibleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

