using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISteppedRangeEntityData))]
	public class UISteppedRangeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UISteppedRangeEntityData>
	{
		public new FrostySdk.Ebx.UISteppedRangeEntityData Data => data as FrostySdk.Ebx.UISteppedRangeEntityData;
		public override string DisplayName => "UISteppedRange";

		public UISteppedRangeEntity(FrostySdk.Ebx.UISteppedRangeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

