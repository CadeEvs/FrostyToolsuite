using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWGetTransformBaseData))]
	public class BWGetTransformBase : LogicEntity, IEntityData<FrostySdk.Ebx.BWGetTransformBaseData>
	{
		public new FrostySdk.Ebx.BWGetTransformBaseData Data => data as FrostySdk.Ebx.BWGetTransformBaseData;
		public override string DisplayName => "BWGetTransformBase";

		public BWGetTransformBase(FrostySdk.Ebx.BWGetTransformBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

