using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueueAmbientActionEntityData))]
	public class QueueAmbientActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueueAmbientActionEntityData>
	{
		public new FrostySdk.Ebx.QueueAmbientActionEntityData Data => data as FrostySdk.Ebx.QueueAmbientActionEntityData;
		public override string DisplayName => "QueueAmbientAction";

		public QueueAmbientActionEntity(FrostySdk.Ebx.QueueAmbientActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

