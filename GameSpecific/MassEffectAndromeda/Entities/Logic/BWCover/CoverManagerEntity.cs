using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverManagerEntityData))]
	public class CoverManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverManagerEntityData>
	{
		public new FrostySdk.Ebx.CoverManagerEntityData Data => data as FrostySdk.Ebx.CoverManagerEntityData;
		public override string DisplayName => "CoverManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CoverManagerEntity(FrostySdk.Ebx.CoverManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

