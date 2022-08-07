using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClearArrayVariableEntityData))]
	public class ClearArrayVariableEntity : WriteVariableBaseEntity, IEntityData<FrostySdk.Ebx.ClearArrayVariableEntityData>
	{
		public new FrostySdk.Ebx.ClearArrayVariableEntityData Data => data as FrostySdk.Ebx.ClearArrayVariableEntityData;
		public override string DisplayName => "ClearArrayVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClearArrayVariableEntity(FrostySdk.Ebx.ClearArrayVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

