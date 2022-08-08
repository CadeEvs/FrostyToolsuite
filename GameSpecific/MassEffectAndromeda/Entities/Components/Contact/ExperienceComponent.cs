using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExperienceComponentData))]
	public class ExperienceComponent : GameComponent, IEntityData<FrostySdk.Ebx.ExperienceComponentData>
	{
		public new FrostySdk.Ebx.ExperienceComponentData Data => data as FrostySdk.Ebx.ExperienceComponentData;
		public override string DisplayName => "ExperienceComponent";

		public ExperienceComponent(FrostySdk.Ebx.ExperienceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

