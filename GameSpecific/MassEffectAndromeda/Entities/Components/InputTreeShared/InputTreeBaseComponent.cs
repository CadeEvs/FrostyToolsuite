using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputTreeBaseComponentData))]
	public class InputTreeBaseComponent : GameComponent, IEntityData<FrostySdk.Ebx.InputTreeBaseComponentData>
	{
		public new FrostySdk.Ebx.InputTreeBaseComponentData Data => data as FrostySdk.Ebx.InputTreeBaseComponentData;
		public override string DisplayName => "InputTreeBaseComponent";

		public InputTreeBaseComponent(FrostySdk.Ebx.InputTreeBaseComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

