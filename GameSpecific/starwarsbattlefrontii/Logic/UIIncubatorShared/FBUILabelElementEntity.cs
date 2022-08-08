using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUILabelElementEntityData))]
	public class FBUILabelElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUILabelElementEntityData>
	{
		public new FrostySdk.Ebx.FBUILabelElementEntityData Data => data as FrostySdk.Ebx.FBUILabelElementEntityData;
		public override string DisplayName => "FBUILabelElement";

		public FBUILabelElementEntity(FrostySdk.Ebx.FBUILabelElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

