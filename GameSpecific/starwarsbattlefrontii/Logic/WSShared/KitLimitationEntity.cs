using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KitLimitationEntityData))]
	public class KitLimitationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KitLimitationEntityData>
	{
		public new FrostySdk.Ebx.KitLimitationEntityData Data => data as FrostySdk.Ebx.KitLimitationEntityData;
		public override string DisplayName => "KitLimitation";

		public KitLimitationEntity(FrostySdk.Ebx.KitLimitationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

