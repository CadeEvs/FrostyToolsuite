using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientEmitTraceBookmarkEntityData))]
	public class ClientEmitTraceBookmarkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientEmitTraceBookmarkEntityData>
	{
		public new FrostySdk.Ebx.ClientEmitTraceBookmarkEntityData Data => data as FrostySdk.Ebx.ClientEmitTraceBookmarkEntityData;
		public override string DisplayName => "ClientEmitTraceBookmark";

		public ClientEmitTraceBookmarkEntity(FrostySdk.Ebx.ClientEmitTraceBookmarkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

