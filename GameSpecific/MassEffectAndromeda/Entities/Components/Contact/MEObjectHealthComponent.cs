using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEObjectHealthComponentData))]
	public class MEObjectHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.MEObjectHealthComponentData>
	{
		public new FrostySdk.Ebx.MEObjectHealthComponentData Data => data as FrostySdk.Ebx.MEObjectHealthComponentData;
		public override string DisplayName => "MEObjectHealthComponent";

		public MEObjectHealthComponent(FrostySdk.Ebx.MEObjectHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

