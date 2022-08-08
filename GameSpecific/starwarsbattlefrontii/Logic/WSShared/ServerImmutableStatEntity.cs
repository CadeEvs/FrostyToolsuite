using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerImmutableStatEntityData))]
	public class ServerImmutableStatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerImmutableStatEntityData>
	{
		public new FrostySdk.Ebx.ServerImmutableStatEntityData Data => data as FrostySdk.Ebx.ServerImmutableStatEntityData;
		public override string DisplayName => "ServerImmutableStat";

		public ServerImmutableStatEntity(FrostySdk.Ebx.ServerImmutableStatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

