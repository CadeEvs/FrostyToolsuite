using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverDangerSourceEntityData))]
	public class CoverDangerSourceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverDangerSourceEntityData>
	{
		public new FrostySdk.Ebx.CoverDangerSourceEntityData Data => data as FrostySdk.Ebx.CoverDangerSourceEntityData;
		public override string DisplayName => "CoverDangerSource";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CoverDangerSourceEntity(FrostySdk.Ebx.CoverDangerSourceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

