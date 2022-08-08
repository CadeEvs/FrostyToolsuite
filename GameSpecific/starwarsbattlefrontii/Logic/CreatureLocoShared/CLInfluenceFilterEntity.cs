using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CLInfluenceFilterEntityData))]
	public class CLInfluenceFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CLInfluenceFilterEntityData>
	{
		public new FrostySdk.Ebx.CLInfluenceFilterEntityData Data => data as FrostySdk.Ebx.CLInfluenceFilterEntityData;
		public override string DisplayName => "CLInfluenceFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CLInfluenceFilterEntity(FrostySdk.Ebx.CLInfluenceFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

