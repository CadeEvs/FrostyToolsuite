using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4InterpolatorEntityData))]
	public class Vec4InterpolatorEntity : PropertyInterpolatorEntity, IEntityData<FrostySdk.Ebx.Vec4InterpolatorEntityData>
	{
		public new FrostySdk.Ebx.Vec4InterpolatorEntityData Data => data as FrostySdk.Ebx.Vec4InterpolatorEntityData;
		public override string DisplayName => "Vec4Interpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec4InterpolatorEntity(FrostySdk.Ebx.Vec4InterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

