using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPCleanupEntityData))]
	public class SPCleanupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPCleanupEntityData>
	{
		public new FrostySdk.Ebx.SPCleanupEntityData Data => data as FrostySdk.Ebx.SPCleanupEntityData;
		public override string DisplayName => "SPCleanup";

		public SPCleanupEntity(FrostySdk.Ebx.SPCleanupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

