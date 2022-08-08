using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StoredStaticMorphBaseComponentData))]
	public class StoredStaticMorphBaseComponent : GameComponent, IEntityData<FrostySdk.Ebx.StoredStaticMorphBaseComponentData>
	{
		public new FrostySdk.Ebx.StoredStaticMorphBaseComponentData Data => data as FrostySdk.Ebx.StoredStaticMorphBaseComponentData;
		public override string DisplayName => "StoredStaticMorphBaseComponent";

		public StoredStaticMorphBaseComponent(FrostySdk.Ebx.StoredStaticMorphBaseComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

