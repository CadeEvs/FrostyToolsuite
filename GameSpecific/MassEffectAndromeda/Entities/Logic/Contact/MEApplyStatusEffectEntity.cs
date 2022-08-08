using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEApplyStatusEffectEntityData))]
	public class MEApplyStatusEffectEntity : MEApplyStatusEffectEntityBase, IEntityData<FrostySdk.Ebx.MEApplyStatusEffectEntityData>
	{
		public new FrostySdk.Ebx.MEApplyStatusEffectEntityData Data => data as FrostySdk.Ebx.MEApplyStatusEffectEntityData;
		public override string DisplayName => "MEApplyStatusEffect";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MEApplyStatusEffectEntity(FrostySdk.Ebx.MEApplyStatusEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

