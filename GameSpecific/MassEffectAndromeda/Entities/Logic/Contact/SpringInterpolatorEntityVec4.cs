using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringInterpolatorEntityVec4Data))]
	public class SpringInterpolatorEntityVec4 : SpringInterpolatorEntity, IEntityData<FrostySdk.Ebx.SpringInterpolatorEntityVec4Data>
	{
		public new FrostySdk.Ebx.SpringInterpolatorEntityVec4Data Data => data as FrostySdk.Ebx.SpringInterpolatorEntityVec4Data;
		public override string DisplayName => "SpringInterpolatorEntityVec4";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpringInterpolatorEntityVec4(FrostySdk.Ebx.SpringInterpolatorEntityVec4Data inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

