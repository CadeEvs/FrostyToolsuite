using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransitionToCSMStateEntityData))]
	public class TransitionToCSMStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransitionToCSMStateEntityData>
	{
		public new FrostySdk.Ebx.TransitionToCSMStateEntityData Data => data as FrostySdk.Ebx.TransitionToCSMStateEntityData;
		public override string DisplayName => "TransitionToCSMState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Context", Direction.In),
				new ConnectionDesc("CasterEntity", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Transition", Direction.In)
			};
		}

        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				if (state != null)
				{
					outHeaderRows.Add($"State: {Path.GetFileName(state.Name)}");
				}
				return outHeaderRows;
            }
        }

        private Assets.BWCSMStateBase state;

		public TransitionToCSMStateEntity(FrostySdk.Ebx.TransitionToCSMStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			FrostySdk.Ebx.CSMStateReference stateRefr = Data.StateReference.GetObjectAs<FrostySdk.Ebx.CSMStateReference>();
			if (stateRefr != null)
			{
				state = LoadedAssetManager.Instance.LoadAsset<Assets.BWCSMStateBase>(this, stateRefr.State);
			}
		}

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(state);
        }
    }
}

