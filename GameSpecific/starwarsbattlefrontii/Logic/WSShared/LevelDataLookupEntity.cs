using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelDataLookupEntityData))]
	public class LevelDataLookupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LevelDataLookupEntityData>
	{
		public new FrostySdk.Ebx.LevelDataLookupEntityData Data => data as FrostySdk.Ebx.LevelDataLookupEntityData;
		public override string DisplayName => "LevelDataLookup";

		public LevelDataLookupEntity(FrostySdk.Ebx.LevelDataLookupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

