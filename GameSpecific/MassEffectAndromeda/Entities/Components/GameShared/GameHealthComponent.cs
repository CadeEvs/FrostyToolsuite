using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameHealthComponentData))]
	public class GameHealthComponent : HealthComponent, IEntityData<FrostySdk.Ebx.GameHealthComponentData>
	{
		public new FrostySdk.Ebx.GameHealthComponentData Data => data as FrostySdk.Ebx.GameHealthComponentData;
		public override string DisplayName => "GameHealthComponent";

		public GameHealthComponent(FrostySdk.Ebx.GameHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

