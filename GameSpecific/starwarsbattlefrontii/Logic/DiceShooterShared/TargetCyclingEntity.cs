using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetCyclingEntityData))]
	public class TargetCyclingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetCyclingEntityData>
	{
		public new FrostySdk.Ebx.TargetCyclingEntityData Data => data as FrostySdk.Ebx.TargetCyclingEntityData;
		public override string DisplayName => "TargetCycling";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TargetCyclingEntity(FrostySdk.Ebx.TargetCyclingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

