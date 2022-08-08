using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIListElementEntityData))]
	public class FBUIListElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIListElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIListElementEntityData Data => data as FrostySdk.Ebx.FBUIListElementEntityData;
		public override string DisplayName => "FBUIListElement";

		public FBUIListElementEntity(FrostySdk.Ebx.FBUIListElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

