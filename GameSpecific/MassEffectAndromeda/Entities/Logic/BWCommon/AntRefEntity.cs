using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntRefEntityData))]
	public class AntRefEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AntRefEntityData>
	{
		public new FrostySdk.Ebx.AntRefEntityData Data => data as FrostySdk.Ebx.AntRefEntityData;
		public override string DisplayName => "AntRef";

		public AntRefEntity(FrostySdk.Ebx.AntRefEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

