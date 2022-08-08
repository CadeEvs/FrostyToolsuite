using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEMultiTiersEntityData))]
	public class MEMultiTiersEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEMultiTiersEntityData>
	{
		public new FrostySdk.Ebx.MEMultiTiersEntityData Data => data as FrostySdk.Ebx.MEMultiTiersEntityData;
		public override string DisplayName => "MEMultiTiers";

		public MEMultiTiersEntity(FrostySdk.Ebx.MEMultiTiersEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

