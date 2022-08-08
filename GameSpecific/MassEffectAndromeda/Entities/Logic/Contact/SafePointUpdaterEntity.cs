using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SafePointUpdaterEntityData))]
	public class SafePointUpdaterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SafePointUpdaterEntityData>
	{
		public new FrostySdk.Ebx.SafePointUpdaterEntityData Data => data as FrostySdk.Ebx.SafePointUpdaterEntityData;
		public override string DisplayName => "SafePointUpdater";

		public SafePointUpdaterEntity(FrostySdk.Ebx.SafePointUpdaterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

