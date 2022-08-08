using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnComboDetonationEntityData))]
	public class OnComboDetonationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnComboDetonationEntityData>
	{
		public new FrostySdk.Ebx.OnComboDetonationEntityData Data => data as FrostySdk.Ebx.OnComboDetonationEntityData;
		public override string DisplayName => "OnComboDetonation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OnComboDetonationEntity(FrostySdk.Ebx.OnComboDetonationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

