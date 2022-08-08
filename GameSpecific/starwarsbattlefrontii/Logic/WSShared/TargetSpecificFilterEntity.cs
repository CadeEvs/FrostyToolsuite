using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetSpecificFilterEntityData))]
	public class TargetSpecificFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetSpecificFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetSpecificFilterEntityData Data => data as FrostySdk.Ebx.TargetSpecificFilterEntityData;
		public override string DisplayName => "TargetSpecificFilter";

		public TargetSpecificFilterEntity(FrostySdk.Ebx.TargetSpecificFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

