using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TurretFirePatternEntityData))]
	public class TurretFirePatternEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TurretFirePatternEntityData>
	{
		public new FrostySdk.Ebx.TurretFirePatternEntityData Data => data as FrostySdk.Ebx.TurretFirePatternEntityData;
		public override string DisplayName => "TurretFirePattern";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TurretFirePatternEntity(FrostySdk.Ebx.TurretFirePatternEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

