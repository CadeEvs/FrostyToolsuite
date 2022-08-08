using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicQueryFilterEntityData))]
	public class DynamicQueryFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicQueryFilterEntityData>
	{
		public new FrostySdk.Ebx.DynamicQueryFilterEntityData Data => data as FrostySdk.Ebx.DynamicQueryFilterEntityData;
		public override string DisplayName => "DynamicQueryFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DynamicQueryFilterEntity(FrostySdk.Ebx.DynamicQueryFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

