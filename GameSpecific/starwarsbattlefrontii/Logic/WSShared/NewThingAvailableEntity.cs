using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NewThingAvailableEntityData))]
	public class NewThingAvailableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NewThingAvailableEntityData>
	{
		public new FrostySdk.Ebx.NewThingAvailableEntityData Data => data as FrostySdk.Ebx.NewThingAvailableEntityData;
		public override string DisplayName => "NewThingAvailable";

		public NewThingAvailableEntity(FrostySdk.Ebx.NewThingAvailableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

