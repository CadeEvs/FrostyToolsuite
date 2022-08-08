using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIStaticTextureElementEntityData))]
	public class FBUIStaticTextureElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIStaticTextureElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIStaticTextureElementEntityData Data => data as FrostySdk.Ebx.FBUIStaticTextureElementEntityData;
		public override string DisplayName => "FBUIStaticTextureElement";

		public FBUIStaticTextureElementEntity(FrostySdk.Ebx.FBUIStaticTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

