using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WriteVariableBaseEntityData))]
	public class WriteVariableBaseEntity : VariableBaseEntity, IEntityData<FrostySdk.Ebx.WriteVariableBaseEntityData>
	{
		public new FrostySdk.Ebx.WriteVariableBaseEntityData Data => data as FrostySdk.Ebx.WriteVariableBaseEntityData;
		public override string DisplayName => "WriteVariableBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WriteVariableBaseEntity(FrostySdk.Ebx.WriteVariableBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

