using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SecondaryUserAddedEntityData))]
	public class SecondaryUserAddedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SecondaryUserAddedEntityData>
	{
		public new FrostySdk.Ebx.SecondaryUserAddedEntityData Data => data as FrostySdk.Ebx.SecondaryUserAddedEntityData;
		public override string DisplayName => "SecondaryUserAdded";

		public SecondaryUserAddedEntity(FrostySdk.Ebx.SecondaryUserAddedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

