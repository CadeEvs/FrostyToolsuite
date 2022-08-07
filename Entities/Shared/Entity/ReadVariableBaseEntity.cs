using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadVariableBaseEntityData))]
	public class ReadVariableBaseEntity : VariableBaseEntity, IEntityData<FrostySdk.Ebx.ReadVariableBaseEntityData>
	{
		public new FrostySdk.Ebx.ReadVariableBaseEntityData Data => data as FrostySdk.Ebx.ReadVariableBaseEntityData;
		public override string DisplayName => "ReadVariableBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReadVariableBaseEntity(FrostySdk.Ebx.ReadVariableBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

