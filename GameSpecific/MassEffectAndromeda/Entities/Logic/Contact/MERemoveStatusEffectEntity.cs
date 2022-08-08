using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MERemoveStatusEffectEntityData))]
	public class MERemoveStatusEffectEntity : MEApplyStatusEffectEntityBase, IEntityData<FrostySdk.Ebx.MERemoveStatusEffectEntityData>
	{
		public new FrostySdk.Ebx.MERemoveStatusEffectEntityData Data => data as FrostySdk.Ebx.MERemoveStatusEffectEntityData;
		public override string DisplayName => "MERemoveStatusEffect";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MERemoveStatusEffectEntity(FrostySdk.Ebx.MERemoveStatusEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

