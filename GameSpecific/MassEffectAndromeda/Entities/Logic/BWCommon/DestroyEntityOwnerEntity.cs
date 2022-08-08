using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestroyEntityOwnerEntityData))]
	public class DestroyEntityOwnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DestroyEntityOwnerEntityData>
	{
		public new FrostySdk.Ebx.DestroyEntityOwnerEntityData Data => data as FrostySdk.Ebx.DestroyEntityOwnerEntityData;
		public override string DisplayName => "DestroyEntityOwner";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DestroyEntityOwnerEntity(FrostySdk.Ebx.DestroyEntityOwnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

