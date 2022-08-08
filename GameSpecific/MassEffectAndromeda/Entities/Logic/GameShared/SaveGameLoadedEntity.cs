using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SaveGameLoadedEntityData))]
	public class SaveGameLoadedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SaveGameLoadedEntityData>
	{
		public new FrostySdk.Ebx.SaveGameLoadedEntityData Data => data as FrostySdk.Ebx.SaveGameLoadedEntityData;
		public override string DisplayName => "SaveGameLoaded";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnGameLoaded", Direction.Out)
			};
		}

		public SaveGameLoadedEntity(FrostySdk.Ebx.SaveGameLoadedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

