using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringInterpolatorEntityVec2Data))]
	public class SpringInterpolatorEntityVec2 : SpringInterpolatorEntity, IEntityData<FrostySdk.Ebx.SpringInterpolatorEntityVec2Data>
	{
		public new FrostySdk.Ebx.SpringInterpolatorEntityVec2Data Data => data as FrostySdk.Ebx.SpringInterpolatorEntityVec2Data;
		public override string DisplayName => "SpringInterpolatorEntityVec2";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpringInterpolatorEntityVec2(FrostySdk.Ebx.SpringInterpolatorEntityVec2Data inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

