using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3InterpolatorEntityData))]
	public class Vec3InterpolatorEntity : PropertyInterpolatorEntity, IEntityData<FrostySdk.Ebx.Vec3InterpolatorEntityData>
	{
		public new FrostySdk.Ebx.Vec3InterpolatorEntityData Data => data as FrostySdk.Ebx.Vec3InterpolatorEntityData;
		public override string DisplayName => "Vec3Interpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec3InterpolatorEntity(FrostySdk.Ebx.Vec3InterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

