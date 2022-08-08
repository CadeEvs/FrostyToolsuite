using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESaveEntityData))]
	public class MESaveEntity : SaveEntity, IEntityData<FrostySdk.Ebx.MESaveEntityData>
	{
		public new FrostySdk.Ebx.MESaveEntityData Data => data as FrostySdk.Ebx.MESaveEntityData;
		public override string DisplayName => "MESave";

		public MESaveEntity(FrostySdk.Ebx.MESaveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

