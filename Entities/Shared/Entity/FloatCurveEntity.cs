using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatCurveEntityData))]
	public class FloatCurveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatCurveEntityData>
	{
		public new FrostySdk.Ebx.FloatCurveEntityData Data => data as FrostySdk.Ebx.FloatCurveEntityData;
		public override string DisplayName => "FloatCurve";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatCurveEntity(FrostySdk.Ebx.FloatCurveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

