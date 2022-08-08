using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntWrapperEntityData))]
	public class IntWrapperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntWrapperEntityData>
	{
		public new FrostySdk.Ebx.IntWrapperEntityData Data => data as FrostySdk.Ebx.IntWrapperEntityData;
		public override string DisplayName => "IntWrapper";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntWrapperEntity(FrostySdk.Ebx.IntWrapperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

