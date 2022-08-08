using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerInputPressEntityData))]
	public class ClientPlayerInputPressEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerInputPressEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerInputPressEntityData Data => data as FrostySdk.Ebx.ClientPlayerInputPressEntityData;
		public override string DisplayName => "ClientPlayerInputPress";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerInputPressEntity(FrostySdk.Ebx.ClientPlayerInputPressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

