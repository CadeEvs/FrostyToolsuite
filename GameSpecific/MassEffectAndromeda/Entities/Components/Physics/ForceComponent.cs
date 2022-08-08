using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceComponentData))]
	public class ForceComponent : Component, IEntityData<FrostySdk.Ebx.ForceComponentData>
	{
		public new FrostySdk.Ebx.ForceComponentData Data => data as FrostySdk.Ebx.ForceComponentData;
		public override string DisplayName => "ForceComponent";

		public ForceComponent(FrostySdk.Ebx.ForceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

