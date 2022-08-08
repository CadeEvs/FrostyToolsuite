using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemPossessEntityData))]
	public class MEItemPossessEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemPossessEntityData>
	{
		public new FrostySdk.Ebx.MEItemPossessEntityData Data => data as FrostySdk.Ebx.MEItemPossessEntityData;
		public override string DisplayName => "MEItemPossess";
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				outEvents.Add(new ConnectionDesc("OnTrue", Direction.Out));
				outEvents.Add(new ConnectionDesc("OnFalse", Direction.Out));
				return outEvents;
			}
		}
        public override IEnumerable<string> HeaderRows
        {
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (item != null)
				{
					outHeaderRows.Add($"Item: {Path.GetFileName(item.Name)}");
				}
				return outHeaderRows;
			}
		}


		private Assets.ItemData item;

        public MEItemPossessEntity(FrostySdk.Ebx.MEItemPossessEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			item = LoadedAssetManager.Instance.LoadAsset<Assets.ItemData>(this, Data.Item);
		}

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(item);
        }
    }
}

