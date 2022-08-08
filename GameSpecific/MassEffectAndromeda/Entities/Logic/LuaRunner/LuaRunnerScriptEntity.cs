using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LuaRunnerScriptEntityData))]
	public class LuaRunnerScriptEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LuaRunnerScriptEntityData>
	{
		public new FrostySdk.Ebx.LuaRunnerScriptEntityData Data => data as FrostySdk.Ebx.LuaRunnerScriptEntityData;
		public override string DisplayName => "LuaRunnerScript";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LuaRunnerScriptEntity(FrostySdk.Ebx.LuaRunnerScriptEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

