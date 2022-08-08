using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceEnemySenseEntityData))]
	public class ForceEnemySenseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ForceEnemySenseEntityData>
	{
		public new FrostySdk.Ebx.ForceEnemySenseEntityData Data => data as FrostySdk.Ebx.ForceEnemySenseEntityData;
		public override string DisplayName => "ForceEnemySense";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Clear", Direction.In)
			};
		}

		public ForceEnemySenseEntity(FrostySdk.Ebx.ForceEnemySenseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

