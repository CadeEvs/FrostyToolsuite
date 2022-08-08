using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintEffectEntityData))]
	public class BlueprintEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.BlueprintEffectEntityData>
	{
		public new FrostySdk.Ebx.BlueprintEffectEntityData Data => data as FrostySdk.Ebx.BlueprintEffectEntityData;

		public BlueprintEffectEntity(FrostySdk.Ebx.BlueprintEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

