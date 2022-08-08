using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StoreProfileValueEntityData))]
	public class StoreProfileValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StoreProfileValueEntityData>
	{
		public new FrostySdk.Ebx.StoreProfileValueEntityData Data => data as FrostySdk.Ebx.StoreProfileValueEntityData;
		public override string DisplayName => "StoreProfileValue";

		public StoreProfileValueEntity(FrostySdk.Ebx.StoreProfileValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

