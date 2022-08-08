using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelGroupHealthComponentData))]
	public class StaticModelGroupHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.StaticModelGroupHealthComponentData>
	{
		public new FrostySdk.Ebx.StaticModelGroupHealthComponentData Data => data as FrostySdk.Ebx.StaticModelGroupHealthComponentData;
		public override string DisplayName => "StaticModelGroupHealthComponent";

		public StaticModelGroupHealthComponent(FrostySdk.Ebx.StaticModelGroupHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

