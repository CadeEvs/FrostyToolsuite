using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KillAllEntityData))]
	public class KillAllEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KillAllEntityData>
	{
		public new FrostySdk.Ebx.KillAllEntityData Data => data as FrostySdk.Ebx.KillAllEntityData;
		public override string DisplayName => "KillAll";

		public KillAllEntity(FrostySdk.Ebx.KillAllEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

