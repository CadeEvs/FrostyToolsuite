using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EarlyReflectionEntityData))]
	public class EarlyReflectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EarlyReflectionEntityData>
	{
		public new FrostySdk.Ebx.EarlyReflectionEntityData Data => data as FrostySdk.Ebx.EarlyReflectionEntityData;
		public override string DisplayName => "EarlyReflection";

		public EarlyReflectionEntity(FrostySdk.Ebx.EarlyReflectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

