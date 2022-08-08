using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OwnerToKitLimitationEntityData))]
	public class OwnerToKitLimitationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OwnerToKitLimitationEntityData>
	{
		public new FrostySdk.Ebx.OwnerToKitLimitationEntityData Data => data as FrostySdk.Ebx.OwnerToKitLimitationEntityData;
		public override string DisplayName => "OwnerToKitLimitation";

		public OwnerToKitLimitationEntity(FrostySdk.Ebx.OwnerToKitLimitationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

