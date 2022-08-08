using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCharacterUtilData))]
	public class ClientCharacterUtil : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCharacterUtilData>
	{
		public new FrostySdk.Ebx.ClientCharacterUtilData Data => data as FrostySdk.Ebx.ClientCharacterUtilData;
		public override string DisplayName => "ClientCharacterUtil";

		public ClientCharacterUtil(FrostySdk.Ebx.ClientCharacterUtilData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

