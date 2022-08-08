using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProjectileDangerZoneComponentData))]
	public class ProjectileDangerZoneComponent : GameComponent, IEntityData<FrostySdk.Ebx.ProjectileDangerZoneComponentData>
	{
		public new FrostySdk.Ebx.ProjectileDangerZoneComponentData Data => data as FrostySdk.Ebx.ProjectileDangerZoneComponentData;
		public override string DisplayName => "ProjectileDangerZoneComponent";

		public ProjectileDangerZoneComponent(FrostySdk.Ebx.ProjectileDangerZoneComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

