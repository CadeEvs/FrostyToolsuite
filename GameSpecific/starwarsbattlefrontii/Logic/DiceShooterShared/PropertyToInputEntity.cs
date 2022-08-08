using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyToInputEntityData))]
	public class PropertyToInputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyToInputEntityData>
	{
		public new FrostySdk.Ebx.PropertyToInputEntityData Data => data as FrostySdk.Ebx.PropertyToInputEntityData;
		public override string DisplayName => "PropertyToInput";

		public PropertyToInputEntity(FrostySdk.Ebx.PropertyToInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

