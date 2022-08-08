using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CLApplyInfluenceEntityData))]
	public class CLApplyInfluenceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CLApplyInfluenceEntityData>
	{
		public new FrostySdk.Ebx.CLApplyInfluenceEntityData Data => data as FrostySdk.Ebx.CLApplyInfluenceEntityData;
		public override string DisplayName => "CLApplyInfluence";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CLApplyInfluenceEntity(FrostySdk.Ebx.CLApplyInfluenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

