using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMissionResultsSelectorWidgetEntityData))]
	public class UIMissionResultsSelectorWidgetEntity : UISelectorWidgetEntity, IEntityData<FrostySdk.Ebx.UIMissionResultsSelectorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIMissionResultsSelectorWidgetEntityData Data => data as FrostySdk.Ebx.UIMissionResultsSelectorWidgetEntityData;
		public override string DisplayName => "UIMissionResultsSelectorWidget";

		public UIMissionResultsSelectorWidgetEntity(FrostySdk.Ebx.UIMissionResultsSelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

