using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputTreeComponentData))]
	public class InputTreeComponent : InputTreeBaseComponent, IEntityData<FrostySdk.Ebx.InputTreeComponentData>
	{
		public new FrostySdk.Ebx.InputTreeComponentData Data => data as FrostySdk.Ebx.InputTreeComponentData;
		public override string DisplayName => "InputTreeComponent";

		public InputTreeComponent(FrostySdk.Ebx.InputTreeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

