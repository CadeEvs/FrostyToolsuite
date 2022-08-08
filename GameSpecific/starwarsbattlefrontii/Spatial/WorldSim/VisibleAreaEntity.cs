using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisibleAreaEntityData))]
	public class VisibleAreaEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.VisibleAreaEntityData>
	{
		public new FrostySdk.Ebx.VisibleAreaEntityData Data => data as FrostySdk.Ebx.VisibleAreaEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VisibleAreaEntity(FrostySdk.Ebx.VisibleAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

