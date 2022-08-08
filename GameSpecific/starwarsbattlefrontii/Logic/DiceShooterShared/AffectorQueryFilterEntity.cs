using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AffectorQueryFilterEntityData))]
	public class AffectorQueryFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AffectorQueryFilterEntityData>
	{
		public new FrostySdk.Ebx.AffectorQueryFilterEntityData Data => data as FrostySdk.Ebx.AffectorQueryFilterEntityData;
		public override string DisplayName => "AffectorQueryFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AffectorQueryFilterEntity(FrostySdk.Ebx.AffectorQueryFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

