using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadProfileValueEntityData))]
	public class ReadProfileValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReadProfileValueEntityData>
	{
		public new FrostySdk.Ebx.ReadProfileValueEntityData Data => data as FrostySdk.Ebx.ReadProfileValueEntityData;
		public override string DisplayName => "ReadProfileValue";

		public ReadProfileValueEntity(FrostySdk.Ebx.ReadProfileValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

