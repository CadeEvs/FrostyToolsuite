using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ModelAnimationEntityData))]
	public class ModelAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ModelAnimationEntityData>
	{
		public new FrostySdk.Ebx.ModelAnimationEntityData Data => data as FrostySdk.Ebx.ModelAnimationEntityData;
		public override string DisplayName => "ModelAnimation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ModelAnimationEntity(FrostySdk.Ebx.ModelAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

