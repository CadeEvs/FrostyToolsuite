using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelStreamingActivatorEntityData))]
	public class SubLevelStreamingActivatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SubLevelStreamingActivatorEntityData>
	{
		public new FrostySdk.Ebx.SubLevelStreamingActivatorEntityData Data => data as FrostySdk.Ebx.SubLevelStreamingActivatorEntityData;
		public override string DisplayName => "SubLevelStreamingActivator";

		public SubLevelStreamingActivatorEntity(FrostySdk.Ebx.SubLevelStreamingActivatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

