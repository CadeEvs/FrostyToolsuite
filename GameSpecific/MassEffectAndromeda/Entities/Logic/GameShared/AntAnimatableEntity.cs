using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntAnimatableEntityData))]
	public class AntAnimatableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AntAnimatableEntityData>
	{
		public new FrostySdk.Ebx.AntAnimatableEntityData Data => data as FrostySdk.Ebx.AntAnimatableEntityData;
		public override string DisplayName => "AntAnimatable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AntAnimatableEntity(FrostySdk.Ebx.AntAnimatableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

