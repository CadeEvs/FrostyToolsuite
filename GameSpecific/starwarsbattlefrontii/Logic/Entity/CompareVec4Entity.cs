using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareVec4EntityData))]
	public class CompareVec4Entity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareVec4EntityData>
	{
		public new FrostySdk.Ebx.CompareVec4EntityData Data => data as FrostySdk.Ebx.CompareVec4EntityData;
		public override string DisplayName => "CompareVec4";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareVec4Entity(FrostySdk.Ebx.CompareVec4EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

