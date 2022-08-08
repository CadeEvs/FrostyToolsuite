using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientSonarEntityData))]
	public class ClientSonarEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientSonarEntityData>
	{
		public new FrostySdk.Ebx.ClientSonarEntityData Data => data as FrostySdk.Ebx.ClientSonarEntityData;
		public override string DisplayName => "ClientSonar";

		public ClientSonarEntity(FrostySdk.Ebx.ClientSonarEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

