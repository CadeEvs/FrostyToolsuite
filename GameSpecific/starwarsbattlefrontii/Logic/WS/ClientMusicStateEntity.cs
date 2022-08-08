using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientMusicStateEntityData))]
	public class ClientMusicStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientMusicStateEntityData>
	{
		public new FrostySdk.Ebx.ClientMusicStateEntityData Data => data as FrostySdk.Ebx.ClientMusicStateEntityData;
		public override string DisplayName => "ClientMusicState";

		public ClientMusicStateEntity(FrostySdk.Ebx.ClientMusicStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

