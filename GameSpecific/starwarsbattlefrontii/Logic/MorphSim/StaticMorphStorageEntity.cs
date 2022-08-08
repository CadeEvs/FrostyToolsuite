using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphStorageEntityData))]
	public class StaticMorphStorageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StaticMorphStorageEntityData>
	{
		public new FrostySdk.Ebx.StaticMorphStorageEntityData Data => data as FrostySdk.Ebx.StaticMorphStorageEntityData;
		public override string DisplayName => "StaticMorphStorage";

		public StaticMorphStorageEntity(FrostySdk.Ebx.StaticMorphStorageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

