using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BindActionEntityData))]
	public class BindActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BindActionEntityData>
	{
		public new FrostySdk.Ebx.BindActionEntityData Data => data as FrostySdk.Ebx.BindActionEntityData;
		public override string DisplayName => "BindAction";

		public BindActionEntity(FrostySdk.Ebx.BindActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

