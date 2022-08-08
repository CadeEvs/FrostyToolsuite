using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProjectileFlyByComponentData))]
	public class ProjectileFlyByComponent : GameComponent, IEntityData<FrostySdk.Ebx.ProjectileFlyByComponentData>
	{
		public new FrostySdk.Ebx.ProjectileFlyByComponentData Data => data as FrostySdk.Ebx.ProjectileFlyByComponentData;
		public override string DisplayName => "ProjectileFlyByComponent";

		public ProjectileFlyByComponent(FrostySdk.Ebx.ProjectileFlyByComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

