using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CrashTheGameEntityData))]
	public class CrashTheGameEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CrashTheGameEntityData>
	{
		public new FrostySdk.Ebx.CrashTheGameEntityData Data => data as FrostySdk.Ebx.CrashTheGameEntityData;
		public override string DisplayName => "CrashTheGame";

		public CrashTheGameEntity(FrostySdk.Ebx.CrashTheGameEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

