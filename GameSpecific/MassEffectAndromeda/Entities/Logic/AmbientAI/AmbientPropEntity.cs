using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientPropEntityData))]
	public class AmbientPropEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AmbientPropEntityData>
	{
		public new FrostySdk.Ebx.AmbientPropEntityData Data => data as FrostySdk.Ebx.AmbientPropEntityData;
		public override string DisplayName => "AmbientProp";

		public AmbientPropEntity(FrostySdk.Ebx.AmbientPropEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

