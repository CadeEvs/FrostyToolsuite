using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPropertyDebugEntityData))]
	public class WSPropertyDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSPropertyDebugEntityData>
	{
		public new FrostySdk.Ebx.WSPropertyDebugEntityData Data => data as FrostySdk.Ebx.WSPropertyDebugEntityData;
		public override string DisplayName => "WSPropertyDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				foreach (FrostySdk.Ebx.PropertyDebugInput debugInput in Data.Inputs)
				{
					outHeaderRows.Add($"{debugInput.NameInternal}: {debugInput.Name}");
				}
				return outHeaderRows;
			}
		}
		
		public WSPropertyDebugEntity(FrostySdk.Ebx.WSPropertyDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

