using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropComponentData))]
	public class PropComponent : Component, IEntityData<FrostySdk.Ebx.PropComponentData>
	{
		public new FrostySdk.Ebx.PropComponentData Data => data as FrostySdk.Ebx.PropComponentData;
		public override string DisplayName => "PropComponent";

		public PropComponent(FrostySdk.Ebx.PropComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

