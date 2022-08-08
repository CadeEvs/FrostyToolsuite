using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringInterpolatorEntityData))]
	public class SpringInterpolatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpringInterpolatorEntityData>
	{
		public new FrostySdk.Ebx.SpringInterpolatorEntityData Data => data as FrostySdk.Ebx.SpringInterpolatorEntityData;
		public override string DisplayName => "SpringInterpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpringInterpolatorEntity(FrostySdk.Ebx.SpringInterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

