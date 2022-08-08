using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HealthComponentData))]
	public class HealthComponent : Component, IEntityData<FrostySdk.Ebx.HealthComponentData>
	{
		public new FrostySdk.Ebx.HealthComponentData Data => data as FrostySdk.Ebx.HealthComponentData;
		public override string DisplayName => "HealthComponent";

		public HealthComponent(FrostySdk.Ebx.HealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

