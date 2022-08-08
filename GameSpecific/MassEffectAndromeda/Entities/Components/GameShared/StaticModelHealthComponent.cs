using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelHealthComponentData))]
	public class StaticModelHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.StaticModelHealthComponentData>
	{
		public new FrostySdk.Ebx.StaticModelHealthComponentData Data => data as FrostySdk.Ebx.StaticModelHealthComponentData;
		public override string DisplayName => "StaticModelHealthComponent";

		public StaticModelHealthComponent(FrostySdk.Ebx.StaticModelHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

