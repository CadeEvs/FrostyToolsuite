using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshSettingsComponentData))]
	public class MeshSettingsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.MeshSettingsComponentData>
	{
		public new FrostySdk.Ebx.MeshSettingsComponentData Data => data as FrostySdk.Ebx.MeshSettingsComponentData;
		public override string DisplayName => "MeshSettingsComponent";

		public MeshSettingsComponent(FrostySdk.Ebx.MeshSettingsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

