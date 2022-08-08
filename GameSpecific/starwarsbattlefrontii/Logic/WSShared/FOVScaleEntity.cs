using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FOVScaleEntityData))]
	public class FOVScaleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FOVScaleEntityData>
	{
		public new FrostySdk.Ebx.FOVScaleEntityData Data => data as FrostySdk.Ebx.FOVScaleEntityData;
		public override string DisplayName => "FOVScale";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FOVScaleEntity(FrostySdk.Ebx.FOVScaleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

