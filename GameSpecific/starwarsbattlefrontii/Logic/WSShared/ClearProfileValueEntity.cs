using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClearProfileValueEntityData))]
	public class ClearProfileValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClearProfileValueEntityData>
	{
		public new FrostySdk.Ebx.ClearProfileValueEntityData Data => data as FrostySdk.Ebx.ClearProfileValueEntityData;
		public override string DisplayName => "ClearProfileValue";

		public ClearProfileValueEntity(FrostySdk.Ebx.ClearProfileValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

