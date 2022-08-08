using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntDrivenComponentData))]
	public class AntDrivenComponent : GameComponent, IEntityData<FrostySdk.Ebx.AntDrivenComponentData>
	{
		public new FrostySdk.Ebx.AntDrivenComponentData Data => data as FrostySdk.Ebx.AntDrivenComponentData;
		public override string DisplayName => "AntDrivenComponent";

		public AntDrivenComponent(FrostySdk.Ebx.AntDrivenComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

