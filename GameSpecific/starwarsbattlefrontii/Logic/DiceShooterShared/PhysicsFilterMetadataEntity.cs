using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsFilterMetadataEntityData))]
	public class PhysicsFilterMetadataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsFilterMetadataEntityData>
	{
		public new FrostySdk.Ebx.PhysicsFilterMetadataEntityData Data => data as FrostySdk.Ebx.PhysicsFilterMetadataEntityData;
		public override string DisplayName => "PhysicsFilterMetadata";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsFilterMetadataEntity(FrostySdk.Ebx.PhysicsFilterMetadataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

