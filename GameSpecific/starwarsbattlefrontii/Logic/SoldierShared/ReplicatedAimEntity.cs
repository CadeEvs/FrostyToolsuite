using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReplicatedAimEntityData))]
	public class ReplicatedAimEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReplicatedAimEntityData>
	{
		public new FrostySdk.Ebx.ReplicatedAimEntityData Data => data as FrostySdk.Ebx.ReplicatedAimEntityData;
		public override string DisplayName => "ReplicatedAim";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReplicatedAimEntity(FrostySdk.Ebx.ReplicatedAimEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

