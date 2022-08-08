using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnvironmentHandlerComponentData))]
	public class EnvironmentHandlerComponent : GameComponent, IEntityData<FrostySdk.Ebx.EnvironmentHandlerComponentData>
	{
		public new FrostySdk.Ebx.EnvironmentHandlerComponentData Data => data as FrostySdk.Ebx.EnvironmentHandlerComponentData;
		public override string DisplayName => "EnvironmentHandlerComponent";

		public EnvironmentHandlerComponent(FrostySdk.Ebx.EnvironmentHandlerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

