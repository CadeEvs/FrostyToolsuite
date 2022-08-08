using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TireTrailManagerEntityData))]
	public class TireTrailManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TireTrailManagerEntityData>
	{
		public new FrostySdk.Ebx.TireTrailManagerEntityData Data => data as FrostySdk.Ebx.TireTrailManagerEntityData;
		public override string DisplayName => "TireTrailManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TireTrailManagerEntity(FrostySdk.Ebx.TireTrailManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

