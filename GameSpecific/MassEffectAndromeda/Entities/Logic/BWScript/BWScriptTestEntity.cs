using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWScriptTestEntityData))]
	public class BWScriptTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BWScriptTestEntityData>
	{
		public new FrostySdk.Ebx.BWScriptTestEntityData Data => data as FrostySdk.Ebx.BWScriptTestEntityData;
		public override string DisplayName => "BWScriptTest";

		public BWScriptTestEntity(FrostySdk.Ebx.BWScriptTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

