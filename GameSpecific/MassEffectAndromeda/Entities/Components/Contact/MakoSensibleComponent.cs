using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MakoSensibleComponentData))]
	public class MakoSensibleComponent : SensibleComponent, IEntityData<FrostySdk.Ebx.MakoSensibleComponentData>
	{
		public new FrostySdk.Ebx.MakoSensibleComponentData Data => data as FrostySdk.Ebx.MakoSensibleComponentData;
		public override string DisplayName => "MakoSensibleComponent";

		public MakoSensibleComponent(FrostySdk.Ebx.MakoSensibleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

