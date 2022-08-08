using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectivesCleanUpEntityData))]
	public class ObjectivesCleanUpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectivesCleanUpEntityData>
	{
		public new FrostySdk.Ebx.ObjectivesCleanUpEntityData Data => data as FrostySdk.Ebx.ObjectivesCleanUpEntityData;
		public override string DisplayName => "ObjectivesCleanUp";

		public ObjectivesCleanUpEntity(FrostySdk.Ebx.ObjectivesCleanUpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

