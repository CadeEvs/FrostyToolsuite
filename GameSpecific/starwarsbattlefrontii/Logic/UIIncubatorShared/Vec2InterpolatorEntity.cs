using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2InterpolatorEntityData))]
	public class Vec2InterpolatorEntity : PropertyInterpolatorEntity, IEntityData<FrostySdk.Ebx.Vec2InterpolatorEntityData>
	{
		public new FrostySdk.Ebx.Vec2InterpolatorEntityData Data => data as FrostySdk.Ebx.Vec2InterpolatorEntityData;
		public override string DisplayName => "Vec2Interpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec2InterpolatorEntity(FrostySdk.Ebx.Vec2InterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

