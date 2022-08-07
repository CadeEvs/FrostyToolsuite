using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareVec3EntityData))]
	public class CompareVec3Entity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareVec3EntityData>
	{
		public new FrostySdk.Ebx.CompareVec3EntityData Data => data as FrostySdk.Ebx.CompareVec3EntityData;
		public override string DisplayName => "CompareVec3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareVec3Entity(FrostySdk.Ebx.CompareVec3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

