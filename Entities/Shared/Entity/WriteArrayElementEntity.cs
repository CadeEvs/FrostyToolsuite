using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WriteArrayElementEntityData))]
	public class WriteArrayElementEntity : WriteVariableBaseEntity, IEntityData<FrostySdk.Ebx.WriteArrayElementEntityData>
	{
		public new FrostySdk.Ebx.WriteArrayElementEntityData Data => data as FrostySdk.Ebx.WriteArrayElementEntityData;
		public override string DisplayName => "WriteArrayElement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WriteArrayElementEntity(FrostySdk.Ebx.WriteArrayElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

