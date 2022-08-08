using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientStatPickerEntityData))]
	public class ClientStatPickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientStatPickerEntityData>
	{
		public new FrostySdk.Ebx.ClientStatPickerEntityData Data => data as FrostySdk.Ebx.ClientStatPickerEntityData;
		public override string DisplayName => "ClientStatPicker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientStatPickerEntity(FrostySdk.Ebx.ClientStatPickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

