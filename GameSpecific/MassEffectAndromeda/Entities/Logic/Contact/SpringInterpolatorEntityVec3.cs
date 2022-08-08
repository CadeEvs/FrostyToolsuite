using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringInterpolatorEntityVec3Data))]
	public class SpringInterpolatorEntityVec3 : SpringInterpolatorEntity, IEntityData<FrostySdk.Ebx.SpringInterpolatorEntityVec3Data>
	{
		public new FrostySdk.Ebx.SpringInterpolatorEntityVec3Data Data => data as FrostySdk.Ebx.SpringInterpolatorEntityVec3Data;
		public override string DisplayName => "SpringInterpolatorEntityVec3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpringInterpolatorEntityVec3(FrostySdk.Ebx.SpringInterpolatorEntityVec3Data inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

