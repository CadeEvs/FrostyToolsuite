using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AddToArrayVariableEntityData))]
	public class AddToArrayVariableEntity : WriteVariableBaseEntity, IEntityData<FrostySdk.Ebx.AddToArrayVariableEntityData>
	{
		public new FrostySdk.Ebx.AddToArrayVariableEntityData Data => data as FrostySdk.Ebx.AddToArrayVariableEntityData;
		public override string DisplayName => "AddToArrayVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AddToArrayVariableEntity(FrostySdk.Ebx.AddToArrayVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

