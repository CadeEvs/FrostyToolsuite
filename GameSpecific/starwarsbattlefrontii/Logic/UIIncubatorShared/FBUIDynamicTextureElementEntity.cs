using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIDynamicTextureElementEntityData))]
	public class FBUIDynamicTextureElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIDynamicTextureElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIDynamicTextureElementEntityData Data => data as FrostySdk.Ebx.FBUIDynamicTextureElementEntityData;
		public override string DisplayName => "FBUIDynamicTextureElement";

		public FBUIDynamicTextureElementEntity(FrostySdk.Ebx.FBUIDynamicTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

