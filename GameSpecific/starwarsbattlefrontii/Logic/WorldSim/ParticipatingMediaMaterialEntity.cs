using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ParticipatingMediaMaterialEntityData))]
	public class ParticipatingMediaMaterialEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ParticipatingMediaMaterialEntityData>
	{
		public new FrostySdk.Ebx.ParticipatingMediaMaterialEntityData Data => data as FrostySdk.Ebx.ParticipatingMediaMaterialEntityData;
		public override string DisplayName => "ParticipatingMediaMaterial";

		public ParticipatingMediaMaterialEntity(FrostySdk.Ebx.ParticipatingMediaMaterialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

