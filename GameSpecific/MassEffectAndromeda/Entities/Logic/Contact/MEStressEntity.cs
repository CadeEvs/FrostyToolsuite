using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStressEntityData))]
	public class MEStressEntity : StressEntity, IEntityData<FrostySdk.Ebx.MEStressEntityData>
	{
		public new FrostySdk.Ebx.MEStressEntityData Data => data as FrostySdk.Ebx.MEStressEntityData;
		public override string DisplayName => "MEStress";

		public MEStressEntity(FrostySdk.Ebx.MEStressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

