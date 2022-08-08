using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkDebugEntityData))]
	public class LinkDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LinkDebugEntityData>
	{
		public new FrostySdk.Ebx.LinkDebugEntityData Data => data as FrostySdk.Ebx.LinkDebugEntityData;
		public override string DisplayName => "LinkDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LinkDebugEntity(FrostySdk.Ebx.LinkDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

