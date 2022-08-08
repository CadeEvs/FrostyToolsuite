using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StatEventReaderEntityData))]
	public class StatEventReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StatEventReaderEntityData>
	{
		public new FrostySdk.Ebx.StatEventReaderEntityData Data => data as FrostySdk.Ebx.StatEventReaderEntityData;
		public override string DisplayName => "StatEventReader";

		public StatEventReaderEntity(FrostySdk.Ebx.StatEventReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

