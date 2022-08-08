using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwarenessTargetEntityData))]
	public class AwarenessTargetEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AwarenessTargetEntityData>
	{
		public new FrostySdk.Ebx.AwarenessTargetEntityData Data => data as FrostySdk.Ebx.AwarenessTargetEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AwarenessTargetEntity(FrostySdk.Ebx.AwarenessTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

