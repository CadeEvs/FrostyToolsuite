using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WriteVariableEntityData))]
	public class WriteVariableEntity : WriteVariableBaseEntity, IEntityData<FrostySdk.Ebx.WriteVariableEntityData>
	{
		public new FrostySdk.Ebx.WriteVariableEntityData Data => data as FrostySdk.Ebx.WriteVariableEntityData;
		public override string DisplayName => "WriteVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WriteVariableEntity(FrostySdk.Ebx.WriteVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

