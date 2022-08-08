using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatInterpolationEntityData))]
	public class FloatInterpolationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatInterpolationEntityData>
	{
		public new FrostySdk.Ebx.FloatInterpolationEntityData Data => data as FrostySdk.Ebx.FloatInterpolationEntityData;
		public override string DisplayName => "FloatInterpolation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatInterpolationEntity(FrostySdk.Ebx.FloatInterpolationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

