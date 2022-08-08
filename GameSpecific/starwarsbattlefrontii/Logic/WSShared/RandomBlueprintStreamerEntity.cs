using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomBlueprintStreamerEntityData))]
	public class RandomBlueprintStreamerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomBlueprintStreamerEntityData>
	{
		public new FrostySdk.Ebx.RandomBlueprintStreamerEntityData Data => data as FrostySdk.Ebx.RandomBlueprintStreamerEntityData;
		public override string DisplayName => "RandomBlueprintStreamer";

		public RandomBlueprintStreamerEntity(FrostySdk.Ebx.RandomBlueprintStreamerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

