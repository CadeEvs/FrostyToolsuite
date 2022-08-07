using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadVariableEntityData))]
	public class ReadVariableEntity : ReadVariableBaseEntity, IEntityData<FrostySdk.Ebx.ReadVariableEntityData>
	{
		public new FrostySdk.Ebx.ReadVariableEntityData Data => data as FrostySdk.Ebx.ReadVariableEntityData;
		public override string DisplayName => "ReadVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReadVariableEntity(FrostySdk.Ebx.ReadVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

