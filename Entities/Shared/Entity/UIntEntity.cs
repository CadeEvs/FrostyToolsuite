using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIntEntityData))]
	public class UIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIntEntityData>
	{
		public new FrostySdk.Ebx.UIntEntityData Data => data as FrostySdk.Ebx.UIntEntityData;
		public override string DisplayName => "UInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIntEntity(FrostySdk.Ebx.UIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

