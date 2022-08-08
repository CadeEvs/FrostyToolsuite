using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceCrashEntityData))]
	public class ForceCrashEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ForceCrashEntityData>
	{
		public new FrostySdk.Ebx.ForceCrashEntityData Data => data as FrostySdk.Ebx.ForceCrashEntityData;
		public override string DisplayName => "ForceCrash";

		public ForceCrashEntity(FrostySdk.Ebx.ForceCrashEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

