using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatInterpolatorEntityData))]
	public class FloatInterpolatorEntity : PropertyInterpolatorEntity, IEntityData<FrostySdk.Ebx.FloatInterpolatorEntityData>
	{
		public new FrostySdk.Ebx.FloatInterpolatorEntityData Data => data as FrostySdk.Ebx.FloatInterpolatorEntityData;
		public override string DisplayName => "FloatInterpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatInterpolatorEntity(FrostySdk.Ebx.FloatInterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

