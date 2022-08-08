using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FactionIdCollectionEntityData))]
	public class FactionIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FactionIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.FactionIdCollectionEntityData Data => data as FrostySdk.Ebx.FactionIdCollectionEntityData;
		public override string DisplayName => "FactionIdCollection";

		public FactionIdCollectionEntity(FrostySdk.Ebx.FactionIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

