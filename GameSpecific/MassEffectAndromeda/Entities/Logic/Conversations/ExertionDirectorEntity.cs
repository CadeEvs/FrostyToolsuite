using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExertionDirectorEntityData))]
	public class ExertionDirectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ExertionDirectorEntityData>
	{
		public new FrostySdk.Ebx.ExertionDirectorEntityData Data => data as FrostySdk.Ebx.ExertionDirectorEntityData;
		public override string DisplayName => "ExertionDirector";

		public ExertionDirectorEntity(FrostySdk.Ebx.ExertionDirectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

