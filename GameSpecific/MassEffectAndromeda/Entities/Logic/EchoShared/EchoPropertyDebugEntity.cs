using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EchoPropertyDebugEntityData))]
	public class EchoPropertyDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EchoPropertyDebugEntityData>
	{
		public new FrostySdk.Ebx.EchoPropertyDebugEntityData Data => data as FrostySdk.Ebx.EchoPropertyDebugEntityData;
		public override string DisplayName => "EchoPropertyDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EchoPropertyDebugEntity(FrostySdk.Ebx.EchoPropertyDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

