using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCharacterManagerData))]
	public class ClientCharacterManager : SingletonEntity, IEntityData<FrostySdk.Ebx.ClientCharacterManagerData>
	{
		public new FrostySdk.Ebx.ClientCharacterManagerData Data => data as FrostySdk.Ebx.ClientCharacterManagerData;
		public override string DisplayName => "ClientCharacterManager";

		public ClientCharacterManager(FrostySdk.Ebx.ClientCharacterManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

