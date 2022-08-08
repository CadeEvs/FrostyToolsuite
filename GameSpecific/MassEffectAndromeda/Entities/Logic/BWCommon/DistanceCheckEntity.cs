using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistanceCheckEntityData))]
	public class DistanceCheckEntity : SimpleEntity, IEntityData<FrostySdk.Ebx.DistanceCheckEntityData>
	{
		public new FrostySdk.Ebx.DistanceCheckEntityData Data => data as FrostySdk.Ebx.DistanceCheckEntityData;
		public override string DisplayName => "DistanceCheck";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DistanceCheckEntity(FrostySdk.Ebx.DistanceCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

