using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphSaveGameStorageEntityData))]
	public class StaticMorphSaveGameStorageEntity : StaticMorphStorageEntity, IEntityData<FrostySdk.Ebx.StaticMorphSaveGameStorageEntityData>
	{
		public new FrostySdk.Ebx.StaticMorphSaveGameStorageEntityData Data => data as FrostySdk.Ebx.StaticMorphSaveGameStorageEntityData;
		public override string DisplayName => "StaticMorphSaveGameStorage";

		public StaticMorphSaveGameStorageEntity(FrostySdk.Ebx.StaticMorphSaveGameStorageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

