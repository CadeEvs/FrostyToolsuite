using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExplosionPackHealthComponentData))]
	public class ExplosionPackHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.ExplosionPackHealthComponentData>
	{
		public new FrostySdk.Ebx.ExplosionPackHealthComponentData Data => data as FrostySdk.Ebx.ExplosionPackHealthComponentData;
		public override string DisplayName => "ExplosionPackHealthComponent";

		public ExplosionPackHealthComponent(FrostySdk.Ebx.ExplosionPackHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

