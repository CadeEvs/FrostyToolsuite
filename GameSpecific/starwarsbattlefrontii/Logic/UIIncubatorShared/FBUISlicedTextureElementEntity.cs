using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUISlicedTextureElementEntityData))]
	public class FBUISlicedTextureElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUISlicedTextureElementEntityData>
	{
		public new FrostySdk.Ebx.FBUISlicedTextureElementEntityData Data => data as FrostySdk.Ebx.FBUISlicedTextureElementEntityData;
		public override string DisplayName => "FBUISlicedTextureElement";

		public FBUISlicedTextureElementEntity(FrostySdk.Ebx.FBUISlicedTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

