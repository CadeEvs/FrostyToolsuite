using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StartEntityData))]
	public class StartEntity : StartPointEntity, IEntityData<FrostySdk.Ebx.StartEntityData>
	{
		public new FrostySdk.Ebx.StartEntityData Data => data as FrostySdk.Ebx.StartEntityData;
		public override string DisplayName => "Start";

		public StartEntity(FrostySdk.Ebx.StartEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

