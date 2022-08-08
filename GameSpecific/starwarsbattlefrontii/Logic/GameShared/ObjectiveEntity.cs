using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveEntityData))]
	public class ObjectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectiveEntityData>
	{
		public new FrostySdk.Ebx.ObjectiveEntityData Data => data as FrostySdk.Ebx.ObjectiveEntityData;
		public override string DisplayName => "Objective";

		public ObjectiveEntity(FrostySdk.Ebx.ObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

