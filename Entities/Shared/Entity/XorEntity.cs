using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.XorEntityData))]
	public class XorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.XorEntityData>
	{
		public new FrostySdk.Ebx.XorEntityData Data => data as FrostySdk.Ebx.XorEntityData;
		public override string DisplayName => "Xor";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public XorEntity(FrostySdk.Ebx.XorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

