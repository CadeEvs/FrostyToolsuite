using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIStealthEntityData))]
	public class AIStealthEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIStealthEntityData>
	{
		public new FrostySdk.Ebx.AIStealthEntityData Data => data as FrostySdk.Ebx.AIStealthEntityData;
		public override string DisplayName => "AIStealth";

		public AIStealthEntity(FrostySdk.Ebx.AIStealthEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

