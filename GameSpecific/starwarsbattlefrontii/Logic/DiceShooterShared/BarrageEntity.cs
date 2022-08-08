using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BarrageEntityData))]
	public class BarrageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BarrageEntityData>
	{
		public new FrostySdk.Ebx.BarrageEntityData Data => data as FrostySdk.Ebx.BarrageEntityData;
		public override string DisplayName => "Barrage";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BarrageEntity(FrostySdk.Ebx.BarrageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

