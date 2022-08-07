using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForEachVariableEntityData))]
	public class ForEachVariableEntity : ReadVariableBaseEntity, IEntityData<FrostySdk.Ebx.ForEachVariableEntityData>
	{
		public new FrostySdk.Ebx.ForEachVariableEntityData Data => data as FrostySdk.Ebx.ForEachVariableEntityData;
		public override string DisplayName => "ForEachVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ForEachVariableEntity(FrostySdk.Ebx.ForEachVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

