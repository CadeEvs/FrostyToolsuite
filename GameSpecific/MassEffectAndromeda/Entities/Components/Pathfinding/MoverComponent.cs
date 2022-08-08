using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MoverComponentData))]
	public class MoverComponent : GameComponent, IEntityData<FrostySdk.Ebx.MoverComponentData>
	{
		public new FrostySdk.Ebx.MoverComponentData Data => data as FrostySdk.Ebx.MoverComponentData;
		public override string DisplayName => "MoverComponent";

		public MoverComponent(FrostySdk.Ebx.MoverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

