using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntEventEntityData))]
	public class AntEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AntEventEntityData>
	{
		public new FrostySdk.Ebx.AntEventEntityData Data => data as FrostySdk.Ebx.AntEventEntityData;
		public override string DisplayName => "AntEvent";

		public AntEventEntity(FrostySdk.Ebx.AntEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

