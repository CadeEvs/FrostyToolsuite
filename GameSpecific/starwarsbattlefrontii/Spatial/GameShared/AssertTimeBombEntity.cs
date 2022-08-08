using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AssertTimeBombEntityData))]
	public class AssertTimeBombEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AssertTimeBombEntityData>
	{
		public new FrostySdk.Ebx.AssertTimeBombEntityData Data => data as FrostySdk.Ebx.AssertTimeBombEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AssertTimeBombEntity(FrostySdk.Ebx.AssertTimeBombEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

