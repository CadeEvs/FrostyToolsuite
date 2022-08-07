using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VariableBaseEntityData))]
	public class VariableBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VariableBaseEntityData>
	{
		public new FrostySdk.Ebx.VariableBaseEntityData Data => data as FrostySdk.Ebx.VariableBaseEntityData;
		public override string DisplayName => "VariableBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VariableBaseEntity(FrostySdk.Ebx.VariableBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

