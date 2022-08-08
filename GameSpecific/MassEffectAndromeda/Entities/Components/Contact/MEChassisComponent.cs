using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEChassisComponentData))]
	public class MEChassisComponent : ChassisComponent, IEntityData<FrostySdk.Ebx.MEChassisComponentData>
	{
		public new FrostySdk.Ebx.MEChassisComponentData Data => data as FrostySdk.Ebx.MEChassisComponentData;
		public override string DisplayName => "MEChassisComponent";

		public MEChassisComponent(FrostySdk.Ebx.MEChassisComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

