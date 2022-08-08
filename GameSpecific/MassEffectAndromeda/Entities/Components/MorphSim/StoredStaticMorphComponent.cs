using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StoredStaticMorphComponentData))]
	public class StoredStaticMorphComponent : StoredStaticMorphBaseComponent, IEntityData<FrostySdk.Ebx.StoredStaticMorphComponentData>
	{
		public new FrostySdk.Ebx.StoredStaticMorphComponentData Data => data as FrostySdk.Ebx.StoredStaticMorphComponentData;
		public override string DisplayName => "StoredStaticMorphComponent";

		public StoredStaticMorphComponent(FrostySdk.Ebx.StoredStaticMorphComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

