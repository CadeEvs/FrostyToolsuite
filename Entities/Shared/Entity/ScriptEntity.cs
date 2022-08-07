using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScriptEntityData))]
	public class ScriptEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScriptEntityData>
	{
		public new FrostySdk.Ebx.ScriptEntityData Data => data as FrostySdk.Ebx.ScriptEntityData;
		public override string DisplayName => "Script";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ScriptEntity(FrostySdk.Ebx.ScriptEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

