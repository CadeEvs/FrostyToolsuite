using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntiAliasComponentData))]
	public class AntiAliasComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.AntiAliasComponentData>
	{
		public new FrostySdk.Ebx.AntiAliasComponentData Data => data as FrostySdk.Ebx.AntiAliasComponentData;
		public override string DisplayName => "AntiAliasComponent";

		public AntiAliasComponent(FrostySdk.Ebx.AntiAliasComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

