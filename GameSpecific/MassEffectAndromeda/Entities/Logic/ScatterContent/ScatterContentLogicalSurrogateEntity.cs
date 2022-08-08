using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentLogicalSurrogateEntityData))]
	public class ScatterContentLogicalSurrogateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentLogicalSurrogateEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentLogicalSurrogateEntityData Data => data as FrostySdk.Ebx.ScatterContentLogicalSurrogateEntityData;
		public override string DisplayName => "ScatterContentLogicalSurrogate";

		public ScatterContentLogicalSurrogateEntity(FrostySdk.Ebx.ScatterContentLogicalSurrogateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

