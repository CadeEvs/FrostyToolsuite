using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatChangedEntityData))]
	public class FloatChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.FloatChangedEntityData>
	{
		public new FrostySdk.Ebx.FloatChangedEntityData Data => data as FrostySdk.Ebx.FloatChangedEntityData;
		public override string DisplayName => "FloatChanged";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatChangedEntity(FrostySdk.Ebx.FloatChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

