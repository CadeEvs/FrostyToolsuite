using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LuaRunnerSharedVarsEntityData))]
	public class LuaRunnerSharedVarsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LuaRunnerSharedVarsEntityData>
	{
		public new FrostySdk.Ebx.LuaRunnerSharedVarsEntityData Data => data as FrostySdk.Ebx.LuaRunnerSharedVarsEntityData;
		public override string DisplayName => "LuaRunnerSharedVars";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LuaRunnerSharedVarsEntity(FrostySdk.Ebx.LuaRunnerSharedVarsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

