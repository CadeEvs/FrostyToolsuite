using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DofReaderEntityData))]
	public class DofReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DofReaderEntityData>
	{
		public new FrostySdk.Ebx.DofReaderEntityData Data => data as FrostySdk.Ebx.DofReaderEntityData;
		public override string DisplayName => "DofReader";

		public DofReaderEntity(FrostySdk.Ebx.DofReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

