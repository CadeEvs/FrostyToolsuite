using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIShootAtTargetsEntityData))]
	public class AIShootAtTargetsEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIShootAtTargetsEntityData>
	{
		public new FrostySdk.Ebx.AIShootAtTargetsEntityData Data => data as FrostySdk.Ebx.AIShootAtTargetsEntityData;
		public override string DisplayName => "AIShootAtTargets";

		public AIShootAtTargetsEntity(FrostySdk.Ebx.AIShootAtTargetsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

