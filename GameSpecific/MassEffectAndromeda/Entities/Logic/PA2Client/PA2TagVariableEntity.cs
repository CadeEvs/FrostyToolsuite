using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2TagVariableEntityData))]
	public class PA2TagVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.PA2TagVariableEntityData>
	{
		public new FrostySdk.Ebx.PA2TagVariableEntityData Data => data as FrostySdk.Ebx.PA2TagVariableEntityData;
		public override string DisplayName => "PA2TagVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PA2TagVariableEntity(FrostySdk.Ebx.PA2TagVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

