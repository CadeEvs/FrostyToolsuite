using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerGenericActionEntityData))]
	public class ControllerGenericActionEntity : ControllerActionEntity, IEntityData<FrostySdk.Ebx.ControllerGenericActionEntityData>
	{
		public new FrostySdk.Ebx.ControllerGenericActionEntityData Data => data as FrostySdk.Ebx.ControllerGenericActionEntityData;
		public override string DisplayName => "ControllerGenericAction";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
		}

		public ControllerGenericActionEntity(FrostySdk.Ebx.ControllerGenericActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

