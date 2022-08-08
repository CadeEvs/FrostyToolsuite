using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerInputRecorderEntityData))]
	public class ClientPlayerInputRecorderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerInputRecorderEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerInputRecorderEntityData Data => data as FrostySdk.Ebx.ClientPlayerInputRecorderEntityData;
		public override string DisplayName => "ClientPlayerInputRecorder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerInputRecorderEntity(FrostySdk.Ebx.ClientPlayerInputRecorderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

