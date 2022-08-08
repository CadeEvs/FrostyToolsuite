using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyChangedEntityData))]
	public class PropertyChangedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyChangedEntityData>
	{
		public new FrostySdk.Ebx.PropertyChangedEntityData Data => data as FrostySdk.Ebx.PropertyChangedEntityData;
		public override string DisplayName => "PropertyChanged";

		public PropertyChangedEntity(FrostySdk.Ebx.PropertyChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

