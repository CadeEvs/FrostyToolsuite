using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEApplyStatusEffectEntityBaseData))]
	public class MEApplyStatusEffectEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.MEApplyStatusEffectEntityBaseData>
	{
		public new FrostySdk.Ebx.MEApplyStatusEffectEntityBaseData Data => data as FrostySdk.Ebx.MEApplyStatusEffectEntityBaseData;
		public override string DisplayName => "MEApplyStatusEffectEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entities", Direction.In)
			};
		}

		public MEApplyStatusEffectEntityBase(FrostySdk.Ebx.MEApplyStatusEffectEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

