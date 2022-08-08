using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllableHealthComponentData))]
	public class ControllableHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.ControllableHealthComponentData>
	{
		public new FrostySdk.Ebx.ControllableHealthComponentData Data => data as FrostySdk.Ebx.ControllableHealthComponentData;
		public override string DisplayName => "ControllableHealthComponent";

		public ControllableHealthComponent(FrostySdk.Ebx.ControllableHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

