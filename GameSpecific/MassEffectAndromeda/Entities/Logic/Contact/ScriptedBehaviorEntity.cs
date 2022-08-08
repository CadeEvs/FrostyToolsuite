using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScriptedBehaviorEntityData))]
	public class ScriptedBehaviorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScriptedBehaviorEntityData>
	{
		public new FrostySdk.Ebx.ScriptedBehaviorEntityData Data => data as FrostySdk.Ebx.ScriptedBehaviorEntityData;
		public override string DisplayName => "ScriptedBehavior";

		public ScriptedBehaviorEntity(FrostySdk.Ebx.ScriptedBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

