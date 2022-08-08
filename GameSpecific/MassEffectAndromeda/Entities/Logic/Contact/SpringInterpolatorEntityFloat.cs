using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringInterpolatorEntityFloatData))]
	public class SpringInterpolatorEntityFloat : SpringInterpolatorEntity, IEntityData<FrostySdk.Ebx.SpringInterpolatorEntityFloatData>
	{
		public new FrostySdk.Ebx.SpringInterpolatorEntityFloatData Data => data as FrostySdk.Ebx.SpringInterpolatorEntityFloatData;
		public override string DisplayName => "SpringInterpolatorEntityFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpringInterpolatorEntityFloat(FrostySdk.Ebx.SpringInterpolatorEntityFloatData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

