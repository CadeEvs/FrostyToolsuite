using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicCastEntityData))]
	public class DynamicCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicCastEntityData>
	{
		public new FrostySdk.Ebx.DynamicCastEntityData Data => data as FrostySdk.Ebx.DynamicCastEntityData;
		public override string DisplayName => "DynamicCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DynamicCastEntity(FrostySdk.Ebx.DynamicCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

