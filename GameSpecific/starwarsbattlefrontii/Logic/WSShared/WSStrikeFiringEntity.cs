using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSStrikeFiringEntityData))]
	public class WSStrikeFiringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSStrikeFiringEntityData>
	{
		public new FrostySdk.Ebx.WSStrikeFiringEntityData Data => data as FrostySdk.Ebx.WSStrikeFiringEntityData;
		public override string DisplayName => "WSStrikeFiring";

		public WSStrikeFiringEntity(FrostySdk.Ebx.WSStrikeFiringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

