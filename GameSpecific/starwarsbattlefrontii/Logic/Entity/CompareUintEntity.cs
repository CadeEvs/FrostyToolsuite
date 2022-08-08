using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareUintEntityData))]
	public class CompareUintEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareUintEntityData>
	{
		public new FrostySdk.Ebx.CompareUintEntityData Data => data as FrostySdk.Ebx.CompareUintEntityData;
		public override string DisplayName => "CompareUint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareUintEntity(FrostySdk.Ebx.CompareUintEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

