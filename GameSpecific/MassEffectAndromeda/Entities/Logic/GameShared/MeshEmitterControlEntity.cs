using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshEmitterControlEntityData))]
	public class MeshEmitterControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MeshEmitterControlEntityData>
	{
		public new FrostySdk.Ebx.MeshEmitterControlEntityData Data => data as FrostySdk.Ebx.MeshEmitterControlEntityData;
		public override string DisplayName => "MeshEmitterControl";

		public MeshEmitterControlEntity(FrostySdk.Ebx.MeshEmitterControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

