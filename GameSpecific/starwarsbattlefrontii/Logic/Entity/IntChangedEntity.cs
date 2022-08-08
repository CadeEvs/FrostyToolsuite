using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntChangedEntityData))]
	public class IntChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.IntChangedEntityData>
	{
		public new FrostySdk.Ebx.IntChangedEntityData Data => data as FrostySdk.Ebx.IntChangedEntityData;
		public override string DisplayName => "IntChanged";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntChangedEntity(FrostySdk.Ebx.IntChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

